﻿using KN_KAMPUS_MERDEKA.Common;
using KN_KAMPUS_MERDEKA.Common.Constant;
using KN_KAMPUS_MERDEKA.Common.Dto.System.Module.Request;
using KN_KAMPUS_MERDEKA.Common.Dto.System.Module.Response;
using KN_KAMPUS_MERDEKA.Common.Entity;
using KN_KAMPUS_MERDEKA.Common.Entity.System;
using KN_KAMPUS_MERDEKA.Common.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using static KN_KAMPUS_MERDEKA.MVC.FilterConfig;

namespace KN_KAMPUS_MERDEKA.MVC.Controllers
{
    public class ModuleController : Controller
    {
        [CheckSessionTimeOut()]
        [CheckAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [ValidateHeaderAntiForgeryTokenAttribute()]
        [CheckSessionTimeOut()]
        public ActionResult InitiateData()
        {
            try
            {
                return Json(clsAPI.CreateResult(true, new ModuleRequest(), string.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                return Json(clsMMainCustomBL.CreateError(ex, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID.ToString(), Request.Url.ToString(), GlobalClass.dLogin.userDat), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost()]
        [ValidateHeaderAntiForgeryTokenAttribute()]
        [CheckSessionTimeOut()]
        public ActionResult getAllModule(int page = 1, int size = 20, string cari = "")
        {
            try
            {
                Pageable<ModuleResponse> pageResponse = mModuleCustomBL.findAll(page, size, cari);
                return Json(clsAPI.CreateResult(true, pageResponse, string.Empty, string.Empty));
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(clsAPI.CreateError(e));
            }
        }


        [HttpPost()]
        [ValidateHeaderAntiForgeryTokenAttribute()]
        [CheckSessionTimeOut()]
        public ActionResult Save(ModuleRequest moduleRequest)
        {
            try
            {
                string userLogin = GlobalClass.dLogin.userDat.intUserID.ToString();
                string txtStatus = string.Empty;
                if (!ModelState.IsValid)
                {
                    foreach (var er in ModelState.Values)
                    {
                        if (er.Errors.Count > 0)
                        {
                            txtStatus = er.Errors[0].ErrorMessage;
                        };
                    }
                    throw new Exception(txtStatus);
                }

                bool bitSuccess = false;

                if (mModuleCustomBL.IsExistMModule(moduleRequest.intModuleID) && moduleRequest.intModuleID != 0)
                {
                    mModule savedModule = mModuleCustomBL.GetMModule(moduleRequest.intModuleID);
                    savedModule.txtDescription = moduleRequest.txtDescription.ToUpper();
                    savedModule.txtUpdatedBy = userLogin;
                    savedModule.dtmUpdatedDate = DateTime.Now;
                    savedModule.txtModuleName = moduleRequest.txtModuleName.ToUpper();
                    //Update 
                    bitSuccess = mModuleCustomBL.UpdateMModule(savedModule, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID, savedModule.txtGUID);
                    txtStatus = mSystemLanguageCustomBL.GetmSystemLanguageValue(clsMMainConstant.MODULE_NAME, clsMMainConstant.LANGUAGE.MSG_INSERT_DATA, GlobalClass.dLogin.txtLangID);
                }
                else
                {
                    mModule module = new mModule(moduleRequest);
                    module.dtmUpdatedDate = DateTime.Now;
                    module.txtUpdatedBy = userLogin;
                    //Create 
                    module.txtGUID = moduleRequest.txtGUID;
                    module.intModuleID = mModuleCustomBL.SaveMModule(module, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID, moduleRequest.txtGUID);
                    txtStatus = mSystemLanguageCustomBL.GetmSystemLanguageValue(clsMMainConstant.MODULE_NAME, clsMMainConstant.LANGUAGE.MSG_INSERT_DATA, GlobalClass.dLogin.txtLangID);
                    bitSuccess = true;
                }
                return Json(clsAPI.CreateResult(bitSuccess, moduleRequest, txtStatus, string.Empty));
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(clsAPI.CreateError(ex));
            }
        }
    }
}
