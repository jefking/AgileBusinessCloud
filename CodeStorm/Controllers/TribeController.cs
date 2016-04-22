// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TribeController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using System.Web.Mvc;
    
    /// <summary>
    /// Tribe Controller
    /// </summary>
    public class TribeController : Controller
    {
        #region Methods
        //
        // GET: /Tribe/
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}