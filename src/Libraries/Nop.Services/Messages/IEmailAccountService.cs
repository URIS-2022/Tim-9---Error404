<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Email account service
    /// </summary>
    public partial interface IEmailAccountService
    {
        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account
        /// </returns>
        Task<EmailAccount> GetEmailAccountByIdAsync(int emailAccountId);

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email accounts list
        /// </returns>
        Task<IList<EmailAccount>> GetAllEmailAccountsAsync();
    }
}
=======
=======
=======
<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Email account service
    /// </summary>
    public partial interface IEmailAccountService
    {
        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account
        /// </returns>
        Task<EmailAccount> GetEmailAccountByIdAsync(int emailAccountId);

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email accounts list
        /// </returns>
        Task<IList<EmailAccount>> GetAllEmailAccountsAsync();
    }
}
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Email account service
    /// </summary>
    public partial interface IEmailAccountService
    {
        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account
        /// </returns>
        Task<EmailAccount> GetEmailAccountByIdAsync(int emailAccountId);

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email accounts list
        /// </returns>
        Task<IList<EmailAccount>> GetAllEmailAccountsAsync();
    }
}
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
