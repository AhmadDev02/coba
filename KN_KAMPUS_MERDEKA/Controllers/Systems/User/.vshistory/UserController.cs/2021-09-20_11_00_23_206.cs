﻿using KN2021_E_RPS.Common;
using KN2021_E_RPS.Common.Constant;
using KN2021_E_RPS.Common.Dto.User.Request;
using KN2021_E_RPS.Common.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static KN2021_E_RPS.MVC.FilterConfig;

namespace KN2021_E_RPS.MVC.Controllers
{
    public class UserController : Controller
    {
        [FilterConfig.CheckSessionTimeOut()]
        [FilterConfig.CheckAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        [ValidateHeaderAntiForgeryTokenAttribute()]
        [FilterConfig.CheckSessionTimeOut()]
        public ActionResult InitiateData()
        {
            try
            {
                return Json(clsAPI.CreateResult(true, new UserRegister(), string.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                return Json(clsMMainCustomBL.CreateError(ex, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID.ToString(), Request.Url.ToString(), GlobalClass.dLogin.userDat), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken()]
        [FilterConfig.CheckSessionTimeOut()]
        public ActionResult GetData(string txtID)
        {
            try
            {
                mUser retDat = new mUser();
                if (!txtID.Equals(string.Empty))
                {
                    retDat = mUserCustomBL.GetMUser(clsGlobal.ParseToInteger(txtID));
                }
                if (retDat == null)
                {
                    retDat = mUserCustomBL.CreateBlankmUser();
                }

                // Untuk menghilangkan EF object.
                var result = JsonConvert.SerializeObject(retDat, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                retDat = mUserCustomBL.parseFromJSON(JObject.Parse(result));

                return Json(clsAPI.CreateResult(true, retDat, string.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                return Json(clsMMainCustomBL.CreateError(ex, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID.ToString(), Request.Url.ToString(), GlobalClass.dLogin.userDat), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost()]
        [ValidateHeaderAntiForgeryToken()]
        [FilterConfig.CheckSessionTimeOut()]
        public ActionResult getBoSample(int page = 1, int size = 20, string cari = "", DateTime start = new DateTime(), DateTime end = new DateTime(), int filter = 1)
        {
            try
            {
                Pageable<BoSampleResponse> pageResponse = clsTrBoSampleCustomBL.findAll(page, size, cari, start, end, filter);
                return Json(clsAPI.CreateResult(true, pageResponse, string.Empty, string.Empty));
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(clsAPI.CreateError(e));
            }
        }
        public ActionResult ResetPassword(string data, string txtGUID)
        {
            try
            {
                bool bitSuccess = false;
                mUser objDat = new mUser();
                string txtStatus = string.Empty;
                if (!data.Equals(string.Empty))
                {
                    JObject jsonDat = JObject.Parse(data);
                    objDat = mUserCustomBL.parseFromJSON(jsonDat);
                    mUserCustomBL.ValidateInput(objDat, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID);
                    if (mUserCustomBL.IsExistMUser(objDat.intUserID))
                    {
                        //Update 
                        bitSuccess = mUserCustomBL.ResetPassword(objDat, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID, txtGUID);
                        txtStatus = "Password berhasil di reset!";
                    }
                    else
                    {
                        throw new Exception("User tidak ditemukan!");
                    }
                }
                return Json(clsAPI.CreateResult(bitSuccess, objDat, txtStatus, string.Empty));
            }
            catch (Exception ex)
            {
                return Json(clsMMainCustomBL.CreateError(ex, GlobalClass.dLogin.userDat.intUserID.ToString(), GlobalClass.dLogin.txtLangID.ToString(), Request.Url.ToString(), GlobalClass.dLogin.userDat), JsonRequestBehavior.AllowGet);
            }
        }


    }
}

