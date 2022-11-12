<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Data;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Fields

        private readonly IRepository<TaxCategory> _taxCategoryRepository;

        #endregion

        #region Ctor

        public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
        {
            _taxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.DeleteAsync(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        public virtual async Task<IList<TaxCategory>> GetAllTaxCategoriesAsync()
        {
            var taxCategories = await _taxCategoryRepository.GetAllAsync(query=>
            {
                return from tc in query
                    orderby tc.DisplayOrder, tc.Id
                    select tc;
            }, cache => default);

            return taxCategories;
        }

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        public virtual async Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId)
        {
            return await _taxCategoryRepository.GetByIdAsync(taxCategoryId, cache => default);
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.InsertAsync(taxCategory);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.UpdateAsync(taxCategory);
        }

        #endregion
    }
=======
=======
=======
<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Data;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Fields

        private readonly IRepository<TaxCategory> _taxCategoryRepository;

        #endregion

        #region Ctor

        public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
        {
            _taxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.DeleteAsync(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        public virtual async Task<IList<TaxCategory>> GetAllTaxCategoriesAsync()
        {
            var taxCategories = await _taxCategoryRepository.GetAllAsync(query=>
            {
                return from tc in query
                    orderby tc.DisplayOrder, tc.Id
                    select tc;
            }, cache => default);

            return taxCategories;
        }

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        public virtual async Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId)
        {
            return await _taxCategoryRepository.GetByIdAsync(taxCategoryId, cache => default);
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.InsertAsync(taxCategory);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.UpdateAsync(taxCategory);
        }

        #endregion
    }
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Tax;
using Nop.Data;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax category service
    /// </summary>
    public partial class TaxCategoryService : ITaxCategoryService
    {
        #region Fields

        private readonly IRepository<TaxCategory> _taxCategoryRepository;

        #endregion

        #region Ctor

        public TaxCategoryService(IRepository<TaxCategory> taxCategoryRepository)
        {
            _taxCategoryRepository = taxCategoryRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.DeleteAsync(taxCategory);
        }

        /// <summary>
        /// Gets all tax categories
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax categories
        /// </returns>
        public virtual async Task<IList<TaxCategory>> GetAllTaxCategoriesAsync()
        {
            var taxCategories = await _taxCategoryRepository.GetAllAsync(query=>
            {
                return from tc in query
                    orderby tc.DisplayOrder, tc.Id
                    select tc;
            }, cache => default);

            return taxCategories;
        }

        /// <summary>
        /// Gets a tax category
        /// </summary>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax category
        /// </returns>
        public virtual async Task<TaxCategory> GetTaxCategoryByIdAsync(int taxCategoryId)
        {
            return await _taxCategoryRepository.GetByIdAsync(taxCategoryId, cache => default);
        }

        /// <summary>
        /// Inserts a tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.InsertAsync(taxCategory);
        }

        /// <summary>
        /// Updates the tax category
        /// </summary>
        /// <param name="taxCategory">Tax category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTaxCategoryAsync(TaxCategory taxCategory)
        {
            await _taxCategoryRepository.UpdateAsync(taxCategory);
        }

        #endregion
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