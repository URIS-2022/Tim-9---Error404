<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    public partial class ProfileController : BasePublicController
    {
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileController(CustomerSettings customerSettings,
            ICustomerService customerService,
            IPermissionService permissionService,
            IProfileModelFactory profileModelFactory)
        {
            _customerSettings = customerSettings;
            _customerService = customerService;
            _permissionService = permissionService;
            _profileModelFactory = profileModelFactory;
        }

        public virtual async Task<IActionResult> Index(int? id, int? pageNumber)
        {
            if (!_customerSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("Homepage");
            }

            var customerId = 0;
            if (id.HasValue)
            {
                customerId = id.Value;
            }

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null || await _customerService.IsGuestAsync(customer))
            {
                return RedirectToRoute("Homepage");
            }

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = AreaNames.Admin }));

            var model = await _profileModelFactory.PrepareProfileIndexModelAsync(customer, pageNumber);
            return View(model);
        }
    }
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    public partial class ProfileController : BasePublicController
    {
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileController(CustomerSettings customerSettings,
            ICustomerService customerService,
            IPermissionService permissionService,
            IProfileModelFactory profileModelFactory)
        {
            _customerSettings = customerSettings;
            _customerService = customerService;
            _permissionService = permissionService;
            _profileModelFactory = profileModelFactory;
        }

        public virtual async Task<IActionResult> Index(int? id, int? pageNumber)
        {
            if (!_customerSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("Homepage");
            }

            var customerId = 0;
            if (id.HasValue)
            {
                customerId = id.Value;
            }

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null || await _customerService.IsGuestAsync(customer))
            {
                return RedirectToRoute("Homepage");
            }

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = AreaNames.Admin }));

            var model = await _profileModelFactory.PrepareProfileIndexModelAsync(customer, pageNumber);
            return View(model);
        }
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework;

namespace Nop.Web.Controllers
{
    public partial class ProfileController : BasePublicController
    {
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly IProfileModelFactory _profileModelFactory;

        public ProfileController(CustomerSettings customerSettings,
            ICustomerService customerService,
            IPermissionService permissionService,
            IProfileModelFactory profileModelFactory)
        {
            _customerSettings = customerSettings;
            _customerService = customerService;
            _permissionService = permissionService;
            _profileModelFactory = profileModelFactory;
        }

        public virtual async Task<IActionResult> Index(int? id, int? pageNumber)
        {
            if (!_customerSettings.AllowViewingProfiles)
            {
                return RedirectToRoute("Homepage");
            }

            var customerId = 0;
            if (id.HasValue)
            {
                customerId = id.Value;
            }

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null || await _customerService.IsGuestAsync(customer))
            {
                return RedirectToRoute("Homepage");
            }

            //display "edit" (manage) link
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                DisplayEditLink(Url.Action("Edit", "Customer", new { id = customer.Id, area = AreaNames.Admin }));

            var model = await _profileModelFactory.PrepareProfileIndexModelAsync(customer, pageNumber);
            return View(model);
        }
    }
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}