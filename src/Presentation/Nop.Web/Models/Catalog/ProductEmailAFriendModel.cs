<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductEmailAFriendModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [NopResourceDisplayName("Products.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
=======
=======
=======
<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductEmailAFriendModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [NopResourceDisplayName("Products.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog
{
    public partial record ProductEmailAFriendModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Products.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [NopResourceDisplayName("Products.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}