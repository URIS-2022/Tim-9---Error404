<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record OrderTotalsModel : BaseNopModel
    {
        public OrderTotalsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
        }
        public bool IsEditable { get; set; }

        public string SubTotal { get; set; }

        public string SubTotalDiscount { get; set; }

        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public bool HideShippingTotal { get; set; }

        public string PaymentMethodAdditionalFee { get; set; }

        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }

        public int WillEarnRewardPoints { get; set; }

        public string OrderTotal { get; set; }

        #region Nested classes

        public partial record TaxRate: BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial record GiftCard : BaseNopEntityModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
            public string Remaining { get; set; }
        }

        #endregion
    }
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record OrderTotalsModel : BaseNopModel
    {
        public OrderTotalsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
        }
        public bool IsEditable { get; set; }

        public string SubTotal { get; set; }

        public string SubTotalDiscount { get; set; }

        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public bool HideShippingTotal { get; set; }

        public string PaymentMethodAdditionalFee { get; set; }

        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }

        public int WillEarnRewardPoints { get; set; }

        public string OrderTotal { get; set; }

        #region Nested classes

        public partial record TaxRate: BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial record GiftCard : BaseNopEntityModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
            public string Remaining { get; set; }
        }

        #endregion
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
﻿using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.ShoppingCart
{
    public partial record OrderTotalsModel : BaseNopModel
    {
        public OrderTotalsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
        }
        public bool IsEditable { get; set; }

        public string SubTotal { get; set; }

        public string SubTotalDiscount { get; set; }

        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public bool HideShippingTotal { get; set; }

        public string PaymentMethodAdditionalFee { get; set; }

        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public IList<GiftCard> GiftCards { get; set; }

        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }

        public int WillEarnRewardPoints { get; set; }

        public string OrderTotal { get; set; }

        #region Nested classes

        public partial record TaxRate: BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial record GiftCard : BaseNopEntityModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
            public string Remaining { get; set; }
        }

        #endregion
    }
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}