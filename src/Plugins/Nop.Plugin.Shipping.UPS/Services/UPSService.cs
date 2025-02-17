﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Services;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS.Services
{
    /// <summary>
    /// Represents UPS service
    /// </summary>
    public class UPSService
    {
        #region Constants

        /// <summary>
        /// Package weight limit (lbs) https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
        /// </summary>
        private const int WEIGHT_LIMIT = 150;

        /// <summary>
        /// Packahe size limit (inches in length and girth combined) https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
        /// </summary>
        private const int SIZE_LIMIT = 165;

        /// <summary>
        /// Package length limit (inches) https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
        /// </summary>
        private const int LENGTH_LIMIT = 108;

        #endregion

        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMeasureService _measureService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWorkContext _workContext;
        private readonly UPSSettings _upsSettings;

        private MeasureDimension _measureDimension;
        private MeasureWeight _measureWeight;
        private MeasureDimension _inchesDimension;
        private MeasureWeight _lbWeight;

        #endregion

        #region Ctor

        public UPSService(CurrencySettings currencySettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            ILogger logger,
            IMeasureService measureService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IWorkContext workContext,
            UPSSettings upsSettings)
        {
            _currencySettings = currencySettings;
            _countryService = countryService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _logger = logger;
            _measureService = measureService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _workContext = workContext;
            _upsSettings = upsSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get the weight limit for the selected weight measure
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the value
        /// </returns>
        private async Task<decimal> GetWeightLimitAsync()
        {
            return await _measureService.ConvertWeightAsync(WEIGHT_LIMIT, _lbWeight, _measureWeight);
        }

        /// <summary>
        /// Get the size limit for the selected dimension measure
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the value
        /// </returns>
        private async Task<decimal> GetSizeLimitAsync()
        {
            return await _measureService.ConvertDimensionAsync(SIZE_LIMIT, _inchesDimension, _measureDimension);
        }

        /// <summary>
        /// Get the length limit for the selected dimension measure
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the value
        /// </returns>
        private async Task<decimal> GetLengthLimitAsync()
        {
            return await _measureService.ConvertDimensionAsync(LENGTH_LIMIT, _inchesDimension, _measureDimension);
        }

        /// <summary>
        /// Gets an attribute value on an enum field value
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <returns>The attribute value</returns>
        private TAttribute GetAttributeValue<TAttribute>(Enum enumValue) where TAttribute : Attribute
        {
            var enumType = enumValue.GetType();
            var enumValueInfo = enumType.GetMember(enumValue.ToString()).FirstOrDefault();
            var attribute = enumValueInfo?.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
            return attribute as TAttribute;
        }

        /// <summary>
        /// Serialize object to XML
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="value">Object to serialize</param>
        /// <returns>XML string</returns>
        private static string ToXml<T>(T value)
        {
            using var writer = new StringWriter();
            using var xmlWriter = new XmlTextWriter(writer) { Formatting = Formatting.Indented };
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(xmlWriter, value);
            return writer.ToString();
        }

        /// <summary>
        /// Get tracking info
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the tracking info
        /// </returns>
        private async Task<UPSTrack.TrackResponse> TrackAsync(UPSTrack.TrackRequest request)
        {
            try
            {
                //create client
                var trackPort = _upsSettings.UseSandbox
                    ? UPSTrack.TrackPortTypeClient.EndpointConfiguration.TrackPort
                    : UPSTrack.TrackPortTypeClient.EndpointConfiguration.ProductionTrackPort;

                using var client = new UPSTrack.TrackPortTypeClient(trackPort);
                //create object to authenticate request
                var security = new UPSTrack.UPSSecurity
                {
                    ServiceAccessToken = new UPSTrack.UPSSecurityServiceAccessToken
                    {
                        AccessLicenseNumber = _upsSettings.AccessKey
                    },
                    UsernameToken = new UPSTrack.UPSSecurityUsernameToken
                    {
                        Username = _upsSettings.Username,
                        Password = _upsSettings.Password
                    }
                };

                //save debug info
                if (_upsSettings.Tracing)
                    await _logger.InformationAsync($"UPS shipment tracking. Request: {ToXml(new UPSTrack.TrackRequest1(security, request))}");

                //try to get response details
                var response = await client.ProcessTrackAsync(security, request);

                //save debug info
                if (_upsSettings.Tracing)
                    await _logger.InformationAsync($"UPS shipment tracking. Response: {ToXml(response)}");

                return response.TrackResponse;
            }
            catch (FaultException<UPSTrack.ErrorDetailType[]> ex)
            {
                //get error details
                var message = ex.Message;
                if (ex.Detail.Any())
                {
                    message = ex.Detail.Aggregate(message, (details, detail) =>
                        $"{details}{Environment.NewLine}{detail.Severity} error: {detail.PrimaryErrorCode?.Description}");
                }

                //rethrow exception
                throw new Exception(message, ex);
            }
        }

        /// <summary>
        /// Create request details to track shipment
        /// </summary>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>Track request details</returns>
        private static UPSTrack.TrackRequest CreateTrackRequest(string trackingNumber)  //promenjeno
        {
            return new UPSTrack.TrackRequest
            {
                Request = new UPSTrack.RequestType
                {
                    //use the RequestOption field to indicate the specific types of information to receive
                    //15 = POD, Signature Image, COD, Receiver Address, All Activity (all that's available)
                    RequestOption = new[] { "15" }
                },
                InquiryNumber = trackingNumber
            };
        }

        /// <summary>
        /// Prepare shipment status event by the passed track activity
        /// </summary>
        /// <param name="activity">Track activity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment status event 
        /// </returns>
        private async Task<ShipmentStatusEvent> PrepareShipmentStatusEventAsync(UPSTrack.ActivityType activity)
        {
            var shipmentStatusEvent = new ShipmentStatusEvent();

            try
            {
                //prepare date
                shipmentStatusEvent.Date = DateTime
                    .ParseExact($"{activity.Date} {activity.Time}", "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);

                //prepare address
                var addressDetails = new List<string>();
                if (!string.IsNullOrEmpty(activity.ActivityLocation?.Address?.CountryCode))
                    addressDetails.Add(activity.ActivityLocation.Address.CountryCode);
                if (!string.IsNullOrEmpty(activity.ActivityLocation?.Address?.StateProvinceCode))
                    addressDetails.Add(activity.ActivityLocation.Address.StateProvinceCode);
                if (!string.IsNullOrEmpty(activity.ActivityLocation?.Address?.City))
                    addressDetails.Add(activity.ActivityLocation.Address.City);
                if (activity.ActivityLocation?.Address?.AddressLine?.Any() ?? false)
                    addressDetails.AddRange(activity.ActivityLocation.Address.AddressLine);
                if (!string.IsNullOrEmpty(activity.ActivityLocation?.Address?.PostalCode))
                    addressDetails.Add(activity.ActivityLocation.Address.PostalCode);

                shipmentStatusEvent.CountryCode = activity.ActivityLocation?.Address?.CountryCode;
                shipmentStatusEvent.Location = string.Join(", ", addressDetails);

                if (activity.Status == null)
                    return shipmentStatusEvent;

                //prepare description
                var eventName = string.Empty;
                switch (activity.Status.Type)
                {
                    case "I":
                        eventName = activity.Status.Code switch
                        {
                            "DP" => "Plugins.Shipping.Tracker.Departed",
                            "EP" => "Plugins.Shipping.Tracker.ExportScanned",
                            "OR" => "Plugins.Shipping.Tracker.OriginScanned",
                            _ => "Plugins.Shipping.Tracker.Arrived",
                        };
                        break;

                    case "X":
                        eventName = "Plugins.Shipping.Tracker.NotDelivered";
                        break;

                    case "M":
                        eventName = "Plugins.Shipping.Tracker.Booked";
                        break;

                    case "D":
                        eventName = "Plugins.Shipping.Tracker.Delivered";
                        break;

                    case "P":
                        eventName = "Plugins.Shipping.Tracker.Pickup";
                        break;
                }
                shipmentStatusEvent.EventName = await _localizationService.GetResourceAsync(eventName);
            }
            catch
            {
                // ignored
            }

            return shipmentStatusEvent;
        }

        /// <summary>
        /// Get rates
        /// </summary>
        /// <param name="request">Request details</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the asynchronous task whose result contains the rates info
        /// </returns>
        private async Task<UPSRate.RateResponse> GetRatesAsync(UPSRate.RateRequest request)
        {
            try
            {
                //create client
                var ratePort = _upsSettings.UseSandbox
                    ? UPSRate.RatePortTypeClient.EndpointConfiguration.RatePort
                    : UPSRate.RatePortTypeClient.EndpointConfiguration.ProductionRatePort;

                using var client = new UPSRate.RatePortTypeClient(ratePort);
                //create object to authenticate request
                var security = new UPSRate.UPSSecurity
                {
                    ServiceAccessToken = new UPSRate.UPSSecurityServiceAccessToken
                    {
                        AccessLicenseNumber = _upsSettings.AccessKey
                    },
                    UsernameToken = new UPSRate.UPSSecurityUsernameToken
                    {
                        Username = _upsSettings.Username,
                        Password = _upsSettings.Password
                    }
                };

                //save debug info
                if (_upsSettings.Tracing)
                    await _logger.InformationAsync($"UPS rates. Request: {ToXml(new UPSRate.RateRequest1(security, request))}");

                //try to get response details
                var response = await client.ProcessRateAsync(security, request);

                //save debug info
                if (_upsSettings.Tracing)
                    await _logger.InformationAsync($"UPS rates. Response: {ToXml(response)}");

                return response.RateResponse;
            }
            catch (FaultException<UPSRate.ErrorDetailType[]> ex)
            {
                //get error details
                var message = ex.Message;
                if (ex.Detail.Any())
                {
                    message = ex.Detail.Aggregate(message, (details, detail) =>
                        $"{details}{Environment.NewLine}{detail.Severity} error: {detail.PrimaryErrorCode?.Description}");
                }

                //rethrow exception
                throw new Exception(message, ex);
            }
        }

        /// <summary>
        /// Create request details to get shipping rates
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request</param>
        /// <param name="saturdayDelivery">Whether to get rates for Saturday Delivery</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rate request details
        /// </returns>
        private async Task<UPSRate.RateRequest> CreateRateRequestAsync(GetShippingOptionRequest shippingOptionRequest, bool saturdayDelivery = false)
        {
            //set request details
            var request = new UPSRate.RateRequest
            {
                Request = new UPSRate.RequestType
                {
                    //used to define the request type
                    //Shop - the server validates the shipment, and returns rates for all UPS products from the ShipFrom to the ShipTo addresses
                    RequestOption = new[] { "Shop" }
                }
            };

            //prepare addresses details
            var stateCodeTo = (await _stateProvinceService.GetStateProvinceByAddressAsync(shippingOptionRequest.ShippingAddress))?.Abbreviation;
            var stateCodeFrom = shippingOptionRequest.StateProvinceFrom?.Abbreviation;
            var countryCodeFrom = (shippingOptionRequest.CountryFrom ?? (await _countryService.GetAllCountriesAsync()).FirstOrDefault())?.TwoLetterIsoCode ?? string.Empty;

            var addressFromDetails = new UPSRate.ShipAddressType
            {
                AddressLine = new[] { shippingOptionRequest.AddressFrom },
                City = shippingOptionRequest.CityFrom,
                StateProvinceCode = stateCodeFrom,
                CountryCode = countryCodeFrom,
                PostalCode = shippingOptionRequest.ZipPostalCodeFrom
            };
            var addressToDetails = new UPSRate.ShipToAddressType
            {
                AddressLine = new[] { shippingOptionRequest.ShippingAddress.Address1, shippingOptionRequest.ShippingAddress.Address2 },
                City = shippingOptionRequest.ShippingAddress.City,
                StateProvinceCode = stateCodeTo,
                CountryCode = (await _countryService.GetCountryByAddressAsync(shippingOptionRequest.ShippingAddress))?.TwoLetterIsoCode,
                PostalCode = shippingOptionRequest.ShippingAddress.ZipPostalCode,
                ResidentialAddressIndicator = string.Empty
            };

            //set shipment details
            request.Shipment = new UPSRate.ShipmentType
            {
                Shipper = new UPSRate.ShipperType
                {
                    ShipperNumber = _upsSettings.AccountNumber,
                    Address = addressFromDetails
                },
                ShipFrom = new UPSRate.ShipFromType
                {
                    Address = addressFromDetails
                },
                ShipTo = new UPSRate.ShipToType
                {
                    Address = addressToDetails
                }
            };

            //set pickup options and customer classification for US shipments
            if (countryCodeFrom.Equals("US", StringComparison.InvariantCultureIgnoreCase))
            {
                request.PickupType = new UPSRate.CodeDescriptionType
                {
                    Code = GetUpsCode(_upsSettings.PickupType)
                };
                request.CustomerClassification = new UPSRate.CodeDescriptionType
                {
                    Code = GetUpsCode(_upsSettings.CustomerClassification)
                };
            }

            //set negotiated rates details
            if (!string.IsNullOrEmpty(_upsSettings.AccountNumber) && !string.IsNullOrEmpty(stateCodeFrom) && !string.IsNullOrEmpty(stateCodeTo))
            {
                request.Shipment.ShipmentRatingOptions = new UPSRate.ShipmentRatingOptionsType
                {
                    NegotiatedRatesIndicator = string.Empty,
                    UserLevelDiscountIndicator = string.Empty
                };
            }

            //set Saturday delivery details
            if (saturdayDelivery)
            {
                request.Shipment.ShipmentServiceOptions = new UPSRate.ShipmentServiceOptionsType
                {
                    SaturdayDeliveryIndicator = string.Empty
                };
            }

            //set packages details
            request.Shipment.Package = _upsSettings.PackingType switch
            {
                PackingType.PackByOneItemPerPackage => (await GetPackagesForOneItemPerPackageAsync(shippingOptionRequest)).ToArray(),
                PackingType.PackByVolume => (await GetPackagesByCubicRootAsync(shippingOptionRequest)).ToArray(),
                _ => (await GetPackagesByDimensionsAsync(shippingOptionRequest)).ToArray()
            };
            return request;
        }

        /// <summary>
        /// Create package details
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="weight">Weight</param>
        /// <param name="insuranceAmount">Insurance amount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the package details
        /// </returns>
        private async Task<UPSRate.PackageType> CreatePackageAsync(decimal width, decimal length, decimal height, decimal weight, decimal insuranceAmount)
        {
            //set package details
            var package = new UPSRate.PackageType
            {
                PackagingType = new UPSRate.CodeDescriptionType
                {
                    Code = GetUpsCode(_upsSettings.PackagingType)
                }
            };

            //set dimensions and weight details
            if (!_upsSettings.PassDimensions)
                width = length = height = 0;
            package.Dimensions = new UPSRate.DimensionsType
            {
                Width = width.ToString("0.00", CultureInfo.InvariantCulture),
                Length = length.ToString("0.00", CultureInfo.InvariantCulture),
                Height = height.ToString("0.00", CultureInfo.InvariantCulture),
                UnitOfMeasurement = new UPSRate.CodeDescriptionType { Code = _upsSettings.DimensionsType }
            };
            package.PackageWeight = new UPSRate.PackageWeightType
            {
                Weight = weight.ToString("0.00", CultureInfo.InvariantCulture),
                UnitOfMeasurement = new UPSRate.CodeDescriptionType { Code = _upsSettings.WeightType },
            };

            //set insurance details
            if (_upsSettings.InsurePackage && insuranceAmount > decimal.Zero)
            {
                var currencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                package.PackageServiceOptions = new UPSRate.PackageServiceOptionsType
                {
                    Insurance = new UPSRate.InsuranceType
                    {
                        BasicFlexibleParcelIndicator = new UPSRate.InsuranceValueType
                        {
                            CurrencyCode = currencyCode,
                            MonetaryValue = insuranceAmount.ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    }
                };
            }

            return package;
        }

        /// <summary>
        /// Create packages (each shopping cart item is a separate package)
        /// </summary>
        /// <param name="shippingOptionRequest">shipping option request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the packages
        /// </returns>
        private async Task<IEnumerable<UPSRate.PackageType>> GetPackagesForOneItemPerPackageAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            return await shippingOptionRequest.Items.SelectManyAwait(async packageItem =>
            {
                //get dimensions and weight of the single item
                var (width, length, height) = await GetDimensionsForSingleItemAsync(packageItem.ShoppingCartItem, packageItem.Product);
                var weight = await GetWeightForSingleItemAsync(packageItem.ShoppingCartItem, shippingOptionRequest.Customer, packageItem.Product);

                var insuranceAmount = 0;
                if (_upsSettings.InsurePackage)
                {
                    //The maximum declared amount per package: 50000 USD.
                    insuranceAmount = Convert.ToInt32(packageItem.Product.Price);
                }

                //create packages according to item quantity
                var package = await CreatePackageAsync(width, length, height, weight, insuranceAmount);

                return Enumerable.Repeat(package, packageItem.GetQuantity());
            }).ToListAsync();
        }

        /// <summary>
        /// Create packages (total dimensions of shopping cart items determines number of packages)
        /// </summary>
        /// <param name="shippingOptionRequest">shipping option request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the packages
        /// </returns>
        private async Task<IEnumerable<UPSRate.PackageType>> GetPackagesByDimensionsAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            //get dimensions and weight of the whole package
            var (width, length, height) = await GetDimensionsAsync(shippingOptionRequest.Items);
            var weight = await GetWeightAsync(shippingOptionRequest);

            //whether the package doesn't exceed the weight and size limits
            var weightLimit = await GetWeightLimitAsync();
            var sizeLimit = await GetSizeLimitAsync();
            if (weight <= weightLimit && GetPackageSize(width, length, height) <= sizeLimit)
            {
                var insuranceAmount = 0;
                if (_upsSettings.InsurePackage)
                {
                    //The maximum declared amount per package: 50000 USD.
                    //use subTotalWithoutDiscount as insured value
                    var cart = shippingOptionRequest.Items.Select(item =>
                    {
                        var shoppingCartItem = item.ShoppingCartItem;
                        shoppingCartItem.Quantity = item.GetQuantity();
                        return shoppingCartItem;
                    }).ToList();
                    var (_, _, subTotalWithoutDiscount, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
                    insuranceAmount = Convert.ToInt32(subTotalWithoutDiscount);
                }

                return new[] { await CreatePackageAsync(width, length, height, weight, insuranceAmount) };
            }

            //get total packages number according to package limits
            var totalPackagesByWeightLimit = weight > weightLimit
                ? Convert.ToInt32(Math.Ceiling(weight / weightLimit))
                : 1;
            var totalPackagesBySizeLimit = GetPackageSize(width, length, height) > sizeLimit
                ? Convert.ToInt32(Math.Ceiling(GetPackageSize(width, length, height) / await GetLengthLimitAsync()))
                : 1;
            var totalPackages = Math.Max(Math.Max(totalPackagesBySizeLimit, totalPackagesByWeightLimit), 1);

            width = Math.Max(width / totalPackages, 1);
            length = Math.Max(length / totalPackages, 1);
            height = Math.Max(height / totalPackages, 1);
            weight = Math.Max(weight / totalPackages, 1);

            var insuranceAmountPerPackage = 0;
            if (_upsSettings.InsurePackage)
            {
                //The maximum declared amount per package: 50000 USD.
                //use subTotalWithoutDiscount as insured value
                var cart = shippingOptionRequest.Items.Select(item =>
                {
                    var shoppingCartItem = item.ShoppingCartItem;
                    shoppingCartItem.Quantity = item.GetQuantity();
                    return shoppingCartItem;
                }).ToList();
                var (_, _, subTotalWithoutDiscount, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
                insuranceAmountPerPackage = Convert.ToInt32(subTotalWithoutDiscount / totalPackages);
            }

            //create packages according to calculated value
            var package = await CreatePackageAsync(width, length, height, weight, insuranceAmountPerPackage);
            return Enumerable.Repeat(package, totalPackages);
        }

        /// <summary>
        /// Create packages (total volume of shopping cart items determines number of packages)
        /// </summary>
        /// <param name="shippingOptionRequest">shipping option request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the packages
        /// </returns>
        private async Task<IEnumerable<UPSRate.PackageType>> GetPackagesByCubicRootAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            //Dimensional weight is based on volume (the amount of space a package occupies in relation to its actual weight). 
            //If the cubic size of package measures three cubic feet (5,184 cubic inches or 84,951 cubic centimetres) or greater, you will be charged the greater of the dimensional weight or the actual weight.
            //This algorithm devides total package volume by the UPS settings PackingPackageVolume so that no package requires dimensional weight; this could result in an under-charge.

            var totalPackagesBySizeLimit = 1;
            var width = 0M;
            var length = 0M;
            var height = 0M;

            //if there is only one item, no need to calculate dimensions
            if (shippingOptionRequest.Items.Count == 1 && shippingOptionRequest.Items.FirstOrDefault().GetQuantity() == 1)
            {
                //get dimensions and weight of the single cubic size of package
                var item = shippingOptionRequest.Items.FirstOrDefault();
                (width, length, height) = await GetDimensionsForSingleItemAsync(item.ShoppingCartItem, item.Product);
            }
            else
            {
                //or try to get them
                var dimension = 0;

                //get total volume of the package
                var totalVolume = await shippingOptionRequest.Items.SumAwaitAsync(async item =>
                {
                    //get dimensions and weight of the single item
                    var (itemWidth, itemLength, itemHeight) = await GetDimensionsForSingleItemAsync(item.ShoppingCartItem, item.Product);
                    return item.GetQuantity() * itemWidth * itemLength * itemHeight;
                });
                if (totalVolume > decimal.Zero)
                {
                    //use default value (in cubic inches) if not specified
                    var packageVolume = _upsSettings.PackingPackageVolume;
                    if (packageVolume <= 0)
                        packageVolume = 5184;

                    //calculate cube root (floor)
                    dimension = Convert.ToInt32(Math.Floor(Math.Pow(Convert.ToDouble(packageVolume), 1.0 / 3.0)));
                    if (GetPackageSize(dimension, dimension, dimension) > await GetSizeLimitAsync())
                        throw new NopException("PackingPackageVolume exceeds max package size");

                    //adjust package volume for dimensions calculated
                    packageVolume = dimension * dimension * dimension;

                    totalPackagesBySizeLimit = Convert.ToInt32(Math.Ceiling(totalVolume / packageVolume));
                }

                width = length = height = dimension;
            }

            //get total packages number according to package limits
            var weight = await GetWeightAsync(shippingOptionRequest);
            var weightLimit = await GetWeightLimitAsync();
            var totalPackagesByWeightLimit = weight > weightLimit
                ? Convert.ToInt32(Math.Ceiling(weight / weightLimit))
                : 1;
            var totalPackages = Math.Max(Math.Max(totalPackagesBySizeLimit, totalPackagesByWeightLimit), 1);

            var insuranceAmountPerPackage = 0;
            if (_upsSettings.InsurePackage)
            {
                //The maximum declared amount per package: 50000 USD.
                //use subTotalWithoutDiscount as insured value
                var cart = shippingOptionRequest.Items.Select(item =>
                {
                    var shoppingCartItem = item.ShoppingCartItem;
                    shoppingCartItem.Quantity = item.GetQuantity();
                    return shoppingCartItem;
                }).ToList();
                var (_, _, subTotalWithoutDiscount, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, false);
                insuranceAmountPerPackage = Convert.ToInt32(subTotalWithoutDiscount / totalPackages);
            }

            //create packages according to calculated value
            var package = await CreatePackageAsync(width, length, height, weight / totalPackages, insuranceAmountPerPackage);
            return Enumerable.Repeat(package, totalPackages);
        }

        /// <summary>
        /// Get dimensions values of the single shopping cart item
        /// </summary>
        /// <param name="item">Shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dimensions values
        /// </returns>
        private async Task<(decimal width, decimal length, decimal height)> GetDimensionsForSingleItemAsync(ShoppingCartItem item, Product product)
        {
            var items = new[] { new GetShippingOptionRequest.PackageItem(item, product, 1) };
            
            return await GetDimensionsAsync(items);
        }

        /// <summary>
        /// Get dimensions values of the package
        /// </summary>
        /// <param name="items">Package items</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the dimensions values
        /// </returns>
        private async Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> items)
        {
            async Task<decimal> convertAndRoundDimensionAsync(decimal dimension)
            {
                dimension = await _measureService.ConvertFromPrimaryMeasureDimensionAsync(dimension, _measureDimension);
                dimension = Convert.ToInt32(Math.Ceiling(dimension));
                return Math.Max(dimension, 1);
            }

            var (width, length, height) = await _shippingService.GetDimensionsAsync(items, true);
            width = await convertAndRoundDimensionAsync(width);
            length = await convertAndRoundDimensionAsync(length);
            height = await convertAndRoundDimensionAsync(height);

            return (width, length, height);
        }

        /// <summary>
        /// Get weight value of the single shopping cart item
        /// </summary>
        /// <param name="item">Shopping cart item</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the weight value
        /// </returns>
        private async Task<decimal> GetWeightForSingleItemAsync(ShoppingCartItem item, Customer customer, Product product)
        {
            var shippingOptionRequest = new GetShippingOptionRequest
            {
                Customer = customer,
                Items = new[] { new GetShippingOptionRequest.PackageItem(item, product, 1) }
            };

            return await GetWeightAsync(shippingOptionRequest);
        }

        /// <summary>
        /// Get weight value of the package
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the weight value
        /// </returns>
        private async Task<decimal> GetWeightAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            var weight = await _shippingService.GetTotalWeightAsync(shippingOptionRequest, ignoreFreeShippedItems: true);
            weight = await _measureService.ConvertFromPrimaryMeasureWeightAsync(weight, _measureWeight);
            weight = Convert.ToInt32(Math.Ceiling(weight));
            return Math.Max(weight, 1);
        }

        /// <summary>
        /// Get package size
        /// </summary>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        /// <param name="width">Width</param>
        /// <returns>Package size</returns>
        private static decimal GetPackageSize(decimal width, decimal length, decimal height)
        {
            //To measure ground packages use the following formula: Length + 2x Width +2x Height. Details: https://www.ups.com/us/en/help-center/packaging-and-supplies/prepare-overize.page
            return length + width * 2 + height * 2;
        }

        /// <summary>
        /// Gets shipping rates
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request details</param>
        /// <param name="saturdayDelivery">Whether to get rates for Saturday Delivery</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping options; errors if exist
        /// </returns>
        private async Task<(IList<ShippingOption> shippingOptions, string error)> GetShippingOptionsAsync(GetShippingOptionRequest shippingOptionRequest,
            bool saturdayDelivery = false)
        {
            try
            {
                //create request details
                var request = await CreateRateRequestAsync(shippingOptionRequest, saturdayDelivery);

                //get rate response
                var rateResponse = await GetRatesAsync(request);

                //prepare shipping options
                return ((await PrepareShippingOptionsAsync(rateResponse)).Select(shippingOption =>
                {
                    //correct option name
                    if (!shippingOption.Name.ToLowerInvariant().StartsWith("ups"))
                        shippingOption.Name = $"UPS {shippingOption.Name}";
                    if (saturdayDelivery)
                        shippingOption.Name = $"{shippingOption.Name} - Saturday Delivery";

                    //add additional handling charge
                    shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;

                    return shippingOption;
                }).ToList(), null);
            }
            catch (Exception exception)
            {
                //log errors
                var message = $"Error while getting UPS rates{Environment.NewLine}{exception.Message}";
                await _logger.ErrorAsync(message, exception, shippingOptionRequest.Customer);

                return (new List<ShippingOption>(), message);
            }
        }

        /// <summary>
        /// Prepare shipping options
        /// </summary>
        /// <param name="rateResponse">Rate response</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping options
        /// </returns>
        private async Task<IEnumerable<ShippingOption>> PrepareShippingOptionsAsync(UPSRate.RateResponse rateResponse)
        {
            var shippingOptions = new List<ShippingOption>();

            if (!rateResponse?.RatedShipment?.Any() ?? true)
                return shippingOptions;

            //prepare offered delivery services
            var servicesCodes = _upsSettings.CarrierServicesOffered.Split(':', StringSplitOptions.RemoveEmptyEntries)
                .Select(idValue => idValue.Trim('[', ']')).ToList();
            var allServices = (await DeliveryService.Standard.ToSelectListAsync(false)).Select(item =>
            {
                var serviceCode = GetUpsCode((DeliveryService)int.Parse(item.Value));
                return new { Name = $"UPS {item.Text?.TrimStart('_')}", Code = serviceCode, Offered = servicesCodes.Contains(serviceCode) };
            }).ToList();

            //get shipping options
            foreach (var rate in rateResponse.RatedShipment)
            {
                //weed out unwanted or unknown service rates
                var serviceCode = rate.Service?.Code;
                var deliveryService = allServices.FirstOrDefault(service => service.Code == serviceCode);
                if (!deliveryService?.Offered ?? true)
                    continue;

                //get rate value
                var regularValue = decimal.TryParse(rate.TotalCharges?.MonetaryValue, NumberStyles.Any, new CultureInfo("en-US"), out var value)
                    ? (decimal?)value
                    : null;
                var negotiatedValue = decimal.TryParse(rate.NegotiatedRateCharges?.TotalCharge?.MonetaryValue, NumberStyles.Any, new CultureInfo("en-US"), out value)
                    ? (decimal?)value
                    : null;
                var monetaryValue = negotiatedValue ?? regularValue;
                if (!monetaryValue.HasValue)
                    continue;

                //parse transit days
                int? transitDays = null;
                if (!string.IsNullOrWhiteSpace(rate.GuaranteedDelivery?.BusinessDaysInTransit))
                {
                    if (int.TryParse(rate.GuaranteedDelivery.BusinessDaysInTransit, out var businessDaysInTransit))
                        transitDays = businessDaysInTransit;
                }

                //add shipping option based on service rate
                shippingOptions.Add(new ShippingOption
                {
                    Rate = monetaryValue.Value,
                    Name = deliveryService.Name,
                    TransitDays = transitDays
                });
            }

            return shippingOptions;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get UPS code of enum value
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>UPS code</returns>
        public string GetUpsCode(Enum enumValue)
        {
            return GetAttributeValue<UPSCodeAttribute>(enumValue)?.Code;
        }

        /// <summary>
        /// Gets all events for a tracking number
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment events
        /// </returns>
        public virtual async Task<IEnumerable<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber)
        {
            try
            {
                //create request details
                var request = CreateTrackRequest(trackingNumber);

                //get tracking info
                var response = await TrackAsync(request);

                if (response.Shipment == null)
                    return null;

                return await response.Shipment
                    .SelectMany(shipment => shipment.Package?
                        .SelectMany(package => package?.Activity)).Where(activity => activity != null)
                    .SelectAwait(async activity => await PrepareShipmentStatusEventAsync(activity)).ToListAsync();
            }
            catch (Exception exception)
            {
                //log errors
                var message = $"Error while getting UPS shipment tracking info - {trackingNumber}{Environment.NewLine}{exception.Message}";
                await _logger.ErrorAsync(message, exception, await _workContext.GetCurrentCustomerAsync());

                return new List<ShipmentStatusEvent>();
            }
        }

        /// <summary>
        /// Gets shipping rates
        /// </summary>
        /// <param name="shippingOptionRequest">Shipping option request details</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        public virtual async Task<GetShippingOptionResponse> GetRatesAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            var weightSystemName = _upsSettings.WeightType switch { "LBS" => "lb", "KGS" => "kg", _ => null };
            _measureWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync(weightSystemName)
                ?? throw new NopException($"UPS shipping service. Could not load \"{weightSystemName}\" measure weight");

            _lbWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync("lb")
                ?? throw new NopException($"UPS shipping service. Could not load 'lb' measure weight (used to find limits)");

            var dimensionSystemName = _upsSettings.DimensionsType switch { "IN" => "inches", "CM" => "centimeters", _ => null };
            _measureDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync(dimensionSystemName)
                ?? throw new NopException($"UPS shipping service. Could not load \"{dimensionSystemName}\" measure dimension");

            _inchesDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync("inches")
                ?? throw new NopException($"UPS shipping service. Could not load 'inches' measure dimension (used to find limits)");

            var response = new GetShippingOptionResponse();

            //get regular rates
            var (shippingOptions, error) = await GetShippingOptionsAsync(shippingOptionRequest);
            response.ShippingOptions = shippingOptions;
            if (!string.IsNullOrEmpty(error))
                response.Errors.Add(error);

            //get rates for Saturday delivery
            if (_upsSettings.SaturdayDeliveryEnabled)
            {
                var (saturdayShippingOptions, saturdayError) = await GetShippingOptionsAsync(shippingOptionRequest, true);
                foreach (var shippingOption in saturdayShippingOptions)
                {
                    response.ShippingOptions.Add(shippingOption);
                }
                if (!string.IsNullOrEmpty(saturdayError))
                    response.Errors.Add(saturdayError);
            }

            if (response.ShippingOptions.Any())
                response.Errors.Clear();

            return response;
        }

        #endregion
    }
}