/*
 * 文件图片
 * mvc
 * 胡锐坚
 * 2017-08-23
 * 
 */
using System; 
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CommonTools.Commonts;
using CommonTools.Models;

namespace CommonTools.Controllers
{
    public class FilePictureController : Controller
    {
        /// <summary>
        /// 上传图片类型
        /// </summary>
        public static string ImageTypes = "gif,jpg,jpeg,bmp,png";
  
        public static string ImageTypesReg = ".gif,.jpg,.jpeg,.bmp,.png";
        /// <summary>
        /// 单张图片最大 5M
        /// </summary>
        public static long ImagemaxSize = 5000000;

        /// <summary>
        /// 图片上传路径
        /// </summary>
        public static string ImageUpload = "~/Upload/Photo/{0}/";

        /// <summary>
        /// 文件上传路径
        /// </summary>
        public static string FileUpload = "~/Upload/File/{0}/";

       /// <summary>
       /// 文件类型
       /// </summary>
        public static string FileTypeis = "txt,zip,gif,jpg,jpeg,bmp,png";

   
        /// <summary>
        /// 多个图片上传
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateImages()
        {
            ViewBag.ImageTypes = ImageTypes;
            ViewBag.ImagemaxSize = ImagemaxSize;
            ViewBag.ImageTypesReg = ImageTypesReg;
            ViewBag.KeyId = "tpscs";
            return View();
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateFileModels()
        {
 
            ViewBag.KeyId = "wjsc12";

            return View();
        }

        /// <summary>
        /// 单个图片上次
        /// </summary>
        /// <returns></returns>
        public ActionResult UpadteImage()
        {
            ViewBag.ImageTypes = ImageTypes;
            ViewBag.ImagemaxSize = ImagemaxSize;
            ViewBag.ImageTypesReg = ImageTypesReg;
            ViewBag.KeyId = "tpsc12";

            return View();
        }
        /// <summary>
        /// 单文件上传（原始）
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateFile()
        {
            ViewBag.ImagemaxSize = ImagemaxSize;
            ViewBag.FileTypeis = FileTypeis;
            ViewBag.KeyId = "wjsc";
            return View();
        }

        [HttpPost]
        public ActionResult FilePost(string keyid)
        {
            var fileurl = "操作失败！";

            var file= Request.Files["file1"];
            if (file != null && file.ContentLength != 0)
            {


                var fileName = file.FileName;
                var extension = Path.GetExtension(fileName);
                if (extension != null)
                {
                    var fileExt = extension.ToLower();


                    if (string.IsNullOrEmpty(fileExt) ||
                        Array.IndexOf(FileTypeis.Split(','), fileExt.Substring(1).ToLower()) == -1)
                    {
                        fileurl = "上传文件扩展名是不允许的扩展名！";
                    }
                    else if (file.InputStream == null || file.InputStream.Length > ImagemaxSize)
                    {
                        fileurl = "上传文件大小超过限制5M！";
                    }
                    else
                    {
                        try
                        {
                            
                            //压缩成3个图 185*185 100*100 50*50 
                            //服务器保存地址
                            var saveUrl = string.Format(FileUpload, keyid);

                            //判断路径是否存在  
                           // 有时候直接用Server.MapPath(string) 调用一个文件比较麻烦，因为不同的目录中调用同一个Server.MapPath(string) 函数就会得到不同的值，特殊的话，就需要通过判断本身目录层次才能获取正确的地址，使用的Request.MapPath(string)就可以调用同一个目录文件。不用做目录判断
                            //var dirPath = Server.MapPath(saveUrl); 
                            var dirPath = Request.MapPath(saveUrl); 
                            if (!Directory.Exists(dirPath))
                            {
                                //上传目录不存在创建
                                Directory.CreateDirectory(dirPath);
                            }

                            if (fileName != null)
                            {
                                var filePath = Path.Combine(dirPath, Path.GetFileName(fileName));
                                file.SaveAs(filePath);
                                fileurl = string.Format("{0}{1}", saveUrl, fileName);
                            }
                           

                        }
                        catch (Exception ex)
                        {
                            fileurl = ex.ToString();
                        }
                    }

                }
            }

            return Content("<script >alert('"+fileurl+"');location.href = '/FilePicture/UpdateFile';</script >", "text/html");
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="uid">系统里面操作数据库用的id</param> 
        /// <param name="id">html上的控件id</param>
        /// <param name="name">图片的名称 eg：tu.jpg</param>
        /// <param name="type">类型 image/jpeg</param>
        /// <param name="lastModifiedDate">不知道什么时间：</param>
        /// <param name="size">文件多大</param>
        /// <param name="file">文件内容</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PhotoUpdate(string uid, string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {
            var jsonUserPhoto = new JsonPhoto();
            // serverurl=string.Format(ConfigNodes.QuestionPhotoPath, uid);
            // servepath = string.Format(servepath, uid);
            //服务器保存地址
            var saveUrl = string.Format(ImageUpload, uid);
            var localPath = Server.MapPath(saveUrl);

            //"WU_FILE_0"
            //覆盖多余的图片
            var sort = id.ToUpper().Replace("WU_FILE_", "");


            if (Request.Files.Count == 0)
            {
                //上传错误
                jsonUserPhoto.Message = "保存失败";
                jsonUserPhoto.State = 102;
                jsonUserPhoto.Result = "";
                return Json(jsonUserPhoto);
            }
            try
            {
               // var ex = Path.GetExtension(file.FileName);
                var filePathName = sort;
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                var waring = "";
                var filname = sort + "yt";
                //保存原图
                var ytserverulr =
                    ImageDealLib.Uploadbystream(file.InputStream, saveUrl, filname, out waring);
                //压缩图片
                var imageUrl = ImageDealLib.Resizepic(ytserverulr, ImageDealLib.ResizeType.Xy, saveUrl,
                    ImageDealLib.ImageType.Jpeg, null, null, ImageDealLib.FileCache.Save, filePathName,
                    out waring); //数据库存储文件路径

                jsonUserPhoto.Message = "上传成功！";
                jsonUserPhoto.State = 0;
                jsonUserPhoto.Result = Url.Content(imageUrl);
                jsonUserPhoto.ImageUrl = imageUrl;
                return Json(jsonUserPhoto);
            }
            catch (Exception ex)
            {
                jsonUserPhoto.Message = "数据库操作失败！";
                jsonUserPhoto.State = 103;
                jsonUserPhoto.Result = ex.Message;
                return Json(jsonUserPhoto);
            }


        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="uid">系统里面操作数据库用的id</param> 
        /// <param name="id">html上的控件id</param>
        /// <param name="name">图片的名称 eg：tu.jpg</param>
        /// <param name="type">类型 image/jpeg</param>
        /// <param name="lastModifiedDate">不知道什么时间：</param>
        /// <param name="size">文件多大</param>
        /// <param name="file">文件内容</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FileUpdate(string uid, string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file)
        {
            var jsonUserPhoto = new JsonPhoto();
            // serverurl=string.Format(ConfigNodes.QuestionPhotoPath, uid);
            // servepath = string.Format(servepath, uid);
            //服务器保存地址
            var saveUrl = string.Format(FileUpload, uid);
            var localPath = Server.MapPath(saveUrl);

            //"WU_FILE_0"
            //覆盖多余的图片
            var sort = id.ToUpper().Replace("WU_FILE_", "");


            if (Request.Files.Count == 0)
            {
                //上传错误
                jsonUserPhoto.Message = "保存失败";
                jsonUserPhoto.State = 102;
                jsonUserPhoto.Result = "";
                return Json(jsonUserPhoto);
            }
            try
            {
                // var ex = Path.GetExtension(file.FileName);
 
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                var waring = "";
                var filname = file.FileName;
                //保存原图
           
                var filePath = Path.Combine(localPath, Path.GetFileName(filname));
                file.SaveAs(filePath);
               var ytserverulr  = string.Format("{0}{1}", saveUrl, filname);

                jsonUserPhoto.Message = "上传成功！";
                jsonUserPhoto.State = 0;
                jsonUserPhoto.Result = Url.Content(ytserverulr);
                jsonUserPhoto.ImageUrl = ytserverulr;
                return Json(jsonUserPhoto);
            }
            catch (Exception ex)
            {
                jsonUserPhoto.Message = "数据库操作失败！";
                jsonUserPhoto.State = 103;
                jsonUserPhoto.Result = ex.Message;
                return Json(jsonUserPhoto);
            }


        }

        /// <summary>
        /// 单文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateFiles()
        {
            ViewBag.KeyId = "wjsc1";
            return View();
        }

        /// <summary>
        /// 上传分块文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            var jsonfile = new JsonPhoto();

            string fileName = Request["name"];
            int index = Convert.ToInt32(Request["chunk"]);//当前分块序号
            var guid = Request["guid"];//前端传来的GUID号
            //服务器保存地址
            var uid = Request["keyid"];
            var saveUrl = string.Format(FileUpload, uid);
            var dir = Server.MapPath(saveUrl);//文件上传目录
             
            dir = Path.Combine(dir, guid);//临时保存分块的目录
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            //分块文件名为索引名
            string filePath = Path.Combine(dir, index.ToString());//分块文件名为索引名 
            
                try
                {
                    var data = Request.Files["file"];//表单中取得分块文件
                    if (data == null)//为null可能是暂停的那一瞬间
                    {
                        jsonfile.Message = "没关系等一等！";
                        jsonfile.State = 103;
                        jsonfile.Result ="";
                        Thread.Sleep(500);
                        return Json(jsonfile);
                    }
                    else
                    {
                        data.SaveAs(filePath);
                        jsonfile.Message = "ok！";
                        jsonfile.State = 200;
                        jsonfile.Result = ""; 
                        return Json(jsonfile);
                    }
                 
                }
                catch (Exception ex)
                {
                    jsonfile.Message = "异常！";
                    jsonfile.State = 103;
                    jsonfile.Result = ex.Message;
                    return Json(jsonfile);
                }
            
        }
        /// <summary>
        /// 合并分块文件并删除分块文件
        /// </summary>
        /// <returns></returns>
        public ActionResult Merge()
        {
            var jsonfile = new JsonPhoto();
            var guid = Request["guid"];//GUID
            //服务器保存地址
            var uid = Request["keyid"];
            var uploadDir = string.Format(FileUpload, uid); //Upload 文件夹
            uploadDir = Server.MapPath(uploadDir);//文件上传目录
            var dir = Path.Combine(uploadDir, guid);//临时文件夹
            var fileName = Request["fileName"];//文件名
            var files = Directory.GetFiles(dir);//获得下面的所有文件
            var finalPath = Path.Combine(uploadDir, fileName);//最终的文件名（demo中保存的是它上传时候的文件名，实际操作视情况而定）
            try
            {
                //保存文件
                var fs = new FileStream(finalPath, FileMode.Create);
                //删除临时分块 
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))//排一下序，保证从0-N Write
                {
                    var bytes = System.IO.File.ReadAllBytes(part);
                    fs.Write(bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete(part);//删除分块
                }
                fs.Flush();
                fs.Close();
                Directory.Delete(dir);//删除文件夹
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            jsonfile.Message = "上传成功！";
            jsonfile.State = 200;
            jsonfile.Result = Url.Content(finalPath);
            jsonfile.ImageUrl = finalPath;
            return Json(jsonfile);//随便返回个值，实际中根据需要返回
        }


        /// <summary>
        /// 图片剪切
        /// </summary>
        /// <returns></returns>
        public ActionResult CropperImage()
        {
        
            ViewBag.ImageTypes = ImageTypes;
            ViewBag.ImagemaxSize = ImagemaxSize;
            ViewBag.KeyId = "tpjq";
            return View();
        }

        /// <summary>
        /// 证件照片处理返回string64 ，只有修改用户证件图片的时候才能使用
        /// js过来的参数名称已固定
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateImgBetyViews()
        {
            var file = Request.Files["avatar_file"];
            var pathname = Request["avatar_src1"]; //可以传递参数
            

            var jsonResult = new JsonPhoto();
            if (file != null && file.ContentLength != 0)
            {
                

                var fileName = file.FileName;
                var extension = Path.GetExtension(fileName);
                if (extension != null)
                {
                    var fileExt = extension.ToLower();


                    if (string.IsNullOrEmpty(fileExt) ||
                        Array.IndexOf(ImageTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
                    {
                        jsonResult.Result = "";
                        jsonResult.State = 301;
                        jsonResult.Message = "上传文件扩展名是不允许的扩展名！";
                    }
                    else if (file.InputStream == null || file.InputStream.Length > ImagemaxSize)
                    {
                        jsonResult.Result = "";
                        jsonResult.State = 401;
                        jsonResult.Message = "上传文件大小超过限制5M！";
                    }
                    else
                    {
                        try
                        {
                            //图片剪切的的参数
                            var jsonStr = Request.Form["avatar_data"];
                            var json = JsonHelper.JsonDeserialize<ImageCrop>(jsonStr);
                             
                            //压缩成3个图 185*185 100*100 50*50 
                            //服务器保存地址
                            var saveUrl = string.Format(ImageUpload, pathname);

                            //判断路径是否存在
                            var dirPath = Server.MapPath(saveUrl);
                            if (!Directory.Exists(dirPath))
                            {
                                //上传目录不存在创建
                                Directory.CreateDirectory(dirPath);
                            }
                            var filname = pathname + "_Yt";
                            var waring = "";
                            //上传服务器原图
                           var savepath = ImageDealLib.Uploadbystream(file.InputStream, saveUrl, filname, out waring);
                            //剪切原图 
                            var filname2 = pathname + "_Jq";
                            savepath = ImageDealLib.Imgcrop(savepath, saveUrl, (int)json.X, (int)json.Y, (int)json.Width, (int)json.Height, filname2,
                                ImageDealLib.FileCache.Save, out waring);


                            //压缩剪切后图，保存压缩图片，并且删除原图
        var filanme185 = pathname + "_185";
                         
                            var picurl185 = ImageDealLib.Resizepic(savepath, ImageDealLib.ResizeType.Xy, saveUrl,
                                ImageDealLib.ImageType.Jpeg, 185, 185, ImageDealLib.FileCache.Save, filanme185,
                                out waring);
                            var filanme100 = pathname + "_100";
                            var picurl100 = ImageDealLib.Resizepic(savepath, ImageDealLib.ResizeType.Xy, saveUrl,
                                ImageDealLib.ImageType.Jpeg, 100, 100, ImageDealLib.FileCache.Save, filanme100,
                                out waring);

                            var filanme50 = pathname + "_50";
                            var picurl50 = ImageDealLib.Resizepic(savepath, ImageDealLib.ResizeType.Xy, saveUrl,
                                ImageDealLib.ImageType.Jpeg, 50, 50, ImageDealLib.FileCache.Save, filanme50,
                                out waring);
                            jsonResult.Result =picurl185;
                            jsonResult.ImageUrl = Url.Content(picurl185);
                            jsonResult.State = 200;
                            jsonResult.Message = "操作成功！";

                        }
                        catch (Exception ex)
                        {
                            jsonResult.State = 201;
                            jsonResult.Message = ex.ToString();
                        }
                    }

                }
            }


            return Json(jsonResult);
        }

    }
}