using LabMaterials.DB;
using LabMaterials.dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client.Extensions.Msal;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Session;

namespace LabMaterials.Pages
{
    public class RequestsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;


        public string lblRequests, lblNewReceivingReport, pagetype = "inbox", inboxClass = "btn-dark text-white", outboxClass = "btn-light";

        public RequestsModel(LabDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public List<ReceivingReport> AllRequest {get; set;}
        public List<ReceivingReport> RequestSent { get; set; }

        public List<Message> InboxList { get; set; }

        public IList<Store> Warehouses { get; set; }
        public List<User> AllUsers {get; set;}
        public List<UserGroup> UserGroups {get; set;}

        public string UserFullName;
        public string UserGroupName;
        public int? UserId;
        public int InboxCount;

        public string ErrorMsg { get; set; }

        public void OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("type"))
            {
                pagetype = HttpContext.Request.Query["type"];

                if (pagetype == "inbox")
                {
                    inboxClass = "btn-dark text-white";
                    outboxClass = "btn-light";
                }
                else
                {
                    inboxClass = "btn-light";
                    outboxClass = "btn-dark text-white";
                }
            }
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");

            this.UserId = HttpContext.Session.GetInt32("UserId");

            AllRequest = dbContext.ReceivingReports.ToList(); 
            AllUsers = dbContext.Users.ToList(); 
            UserGroups = dbContext.UserGroups.ToList();

            RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == UserId)
            .OrderByDescending(r => r.CreatedAt).ToList();
            // ManagerInboxList = dbContext.ReceivingReports
            //     .Where(r => r.KeeperApproval == true)
            //     .ToList();
             Warehouses = dbContext.Stores.ToList(); 
 
            InboxList = dbContext.Messages
                .Where(s => s.RecipientId == UserId).OrderByDescending(s => s.CreatedAt).ToList();

            InboxCount = InboxList.Count();
            

            base.ExtractSessionData();
            FillLables();
        }


         public IActionResult OnPostView([FromForm] string ReportId)
        {
                   var dbContext = new LabDBContext();

            HttpContext.Session.SetString("ReportId", ReportId);
            AllRequest = dbContext.ReceivingReports.ToList(); 

            RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == this.UserId).ToList();


            return RedirectToPage("./ViewReceivingReport");
        }

        public IActionResult OnPostAccept([FromForm] int ReceivingReportId, [FromForm] int MessageId)
        {   
           base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            FillLables();
            var dbContext = new LabDBContext();
                AllRequest = dbContext.ReceivingReports.ToList(); 
                this.UserId = HttpContext.Session.GetInt32("UserId");

                RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == this.UserId).ToList();
                InboxList = dbContext.Messages
                .Where(s => s.RecipientId == this.UserId).ToList();
                var report = dbContext.ReceivingReports.FirstOrDefault(r => r.Id == ReceivingReportId);
          
                if (report != null)
                {   
                    var generalSup = dbContext.Users.FirstOrDefault(u => u.UserId == report.ChiefResponsibleId);

                    var technicalMember = dbContext.Users.FirstOrDefault(u => u.UserId == report.TechnicalMemberId);

                    //If technical Member is logged in than information message is sent to manager and approval message is sent to general Supervisor
                    if(this.UserGroupName == "Technical Member"){
                        var manager = dbContext.Users.FirstOrDefault(u => u.JobNumber == report.RecipientEmployeeId);
                        //update the technical member approval status
                        report.TechnicalMemberApproval = true;
                        report.IsRejectedByTechnicalMember = false;
                        report.TechnicalMemberApprovalDate = DateTime.Now;



                        //message to warehouse manager for information
                        string managerMessage = string.Format("Approved the request for items generated by {0}.", report.CreatedBy);
                        var msgToManager = new Message{
                            ReportId = ReceivingReportId,
                            ReportType = "Receiving",
                            SenderId = this.UserId,
                            RecipientId = manager.UserId,
                            Content = managerMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow

                            

                        };
                        dbContext.Messages.Add(msgToManager);


                        //message to general supervisor for approval
                        //if general Supervisor is assigned.
                        if(generalSup != null){
                            string supMessage = string.Format("Sent Request for Items. Approve the request or add comments.");
                            var msgToSup = new Message{
                                ReportId = ReceivingReportId,
                                ReportType = "Receiving",
                                SenderId = this.UserId,
                                RecipientId = report.ChiefResponsibleId,
                                Content = supMessage,
                                Type = "",
                                CreatedAt = DateTime.UtcNow

                            };
                            dbContext.Messages.Add(msgToSup);

                        }
                        //if general supervisor is not assigned
                        else{
                            //assume it is is approved and set generalsupapproval to true
                            report.GeneralSupApproval = true;
                            report.GeneralSupervisorApprovalDate = DateTime.Now;

                            //send message to keeper
                            string keeperMessage = string.Format("Your request is accepted by {0}(TechnicalMember) and no general supervisor was added.",technicalMember.FullName);
                            var msgToKeeper = new Message{
                                    ReportId = ReceivingReportId,
                                    ReportType = "Receiving",
                                    SenderId = this.UserId,
                                    RecipientId = report.CreatedBy,
                                    Content = keeperMessage,
                                    Type = "",
                                    CreatedAt = DateTime.UtcNow


                            };
                            dbContext.Messages.Add(msgToKeeper);

                        }
                        
                    }
                    else if (this.UserGroupName == "General Supervisor"){
                        //update the general supervisor approval status
                        report.GeneralSupApproval = true;
                        report.IsRejectedByGeneralSupervisor = false;
                        report.GeneralSupervisorApprovalDate = DateTime.Now;


                        string keeperMessage = string.Format("Your request is accepted by {0}(TechnicalMember) and {1}(General Supervisor).",technicalMember.FullName, generalSup.FullName);
                        var msgToKeeper = new Message{
                                ReportId = ReceivingReportId,
                                ReportType = "Receiving",
                                SenderId = this.UserId,
                                RecipientId = report.CreatedBy,
                                Content = keeperMessage,
                                Type = "",
                                CreatedAt = DateTime.UtcNow


                        };
                        dbContext.Messages.Add(msgToKeeper);
                    }
                    var message = dbContext.Messages.FirstOrDefault(m => m.Id == MessageId);

                    if (message != null)
                    {
                        message.Type = "Accepted"; 
                    }
                    dbContext.SaveChanges();
                    
                }
                else{
                    Console.WriteLine("report not found");
                }
                return RedirectToPage();
            }
        

        public async Task<IActionResult> OnPostRejectWithComment([FromForm] int RejectReceivingReportId, [FromForm] string Comment, [FromForm] int RejectMessageId)
        {  
            base.ExtractSessionData();
            FillLables();
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");
            var report = await _context.ReceivingReports.FindAsync(RejectReceivingReportId);
            if (report == null)
            {
                // handle error
                return NotFound();
            }

            if (string.IsNullOrEmpty(Comment))
                ErrorMsg = (Program.Translations["CommentMissing"])[Lang];

            var generalSup = dbContext.Users.FirstOrDefault(u => u.UserId == report.ChiefResponsibleId);

            var technicalMember = dbContext.Users.FirstOrDefault(u => u.UserId == report.TechnicalMemberId);
            var manager = dbContext.Users.FirstOrDefault(u => u.JobNumber == report.RecipientEmployeeId);
            if(this.UserGroupName == "Technical Member"){
                //If Technical Member reject rhe request than send rejeced message to keeper
                report.IsRejectedByTechnicalMember = true;
                if(report.TechnicalMemberApproval == true)
                    report.TechnicalMemberApproval = false;
                string keeperMessage = string.Format("Your request is rejected with comment: {0}", Comment);
                var msgToKeeper = new Message
                {
                    ReportId = RejectReceivingReportId,
                    ReportType = "Receiving",
                    SenderId = this.UserId ,
                    RecipientId = report.CreatedBy, 
                    Content = keeperMessage,
                    Type = "",
                    CreatedAt = DateTime.UtcNow,


                };
                _context.Messages.Add(msgToKeeper);

            }
            else if(this.UserGroupName == "General Supervisor"){
                //If general supervisor reject the request 

                //Send Informaiton mesage to Manager
                report.IsRejectedByGeneralSupervisor = true;
                if(report.GeneralSupApproval == true)
                    report.GeneralSupApproval = false;
                string ManagerMessage = string.Format("Rejected the request for items generated by {0}. with comment: {1}", report.CreatedBy, Comment);
                var msgToManager = new Message
                {
                    ReportId = RejectReceivingReportId,
                    ReportType = "Receiving",
                    SenderId = this.UserId ,
                    RecipientId = report.RecipientEmployeeId, 
                    Content = ManagerMessage,
                    Type = "",
                    CreatedAt = DateTime.UtcNow

                };
                _context.Messages.Add(msgToManager);


                //Send message to technical Member
                string TechMemMessage = string.Format("Rejected the request for items generated by {0}. with comment: {1}", report.CreatedBy, Comment);
                var msgToTechMem = new Message
                {
                    ReportId = RejectReceivingReportId,
                    ReportType = "Receiving",
                    SenderId = this.UserId ,
                    RecipientId = report.TechnicalMemberId, 
                    Content = TechMemMessage,
                    Type = "",
                    CreatedAt = DateTime.UtcNow

                };
                _context.Messages.Add(msgToTechMem);

           

                
            }
           

            this.UserId= HttpContext.Session.GetInt32("UserId");

            if (this.UserId.HasValue)
            {
                report.RejectedById = this.UserId.Value;
            }
            var message = dbContext.Messages.FirstOrDefault(m => m.Id == RejectMessageId);

            if (message != null)
            {
                message.Type = "Accepted"; 
            }
            dbContext.SaveChanges();

           

            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostReply([FromForm] int ReplyReportId, [FromForm] string Reply, [FromForm] int? ReplySender, [FromForm] int ReplyMessageId ){
            base.ExtractSessionData();
            FillLables();
            Console.WriteLine("reply function");
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");
            var report = await _context.ReceivingReports.FindAsync(ReplyReportId);
            if (report == null)
            {
                // handle error
                return NotFound();
            }
            if (string.IsNullOrEmpty(Reply))
                ErrorMsg = (Program.Translations["CommentMissing"])[Lang];

            report.IsReplied = true;
            var message = dbContext.Messages.FirstOrDefault(m => m.Id == ReplyMessageId);
            if (message != null)
            {
                message.Type = "Replied"; 
            }
            dbContext.SaveChanges();
            string ReplyMessage = string.Format("Replied with comment: {0}", Reply);
            var msgToTechMem = new Message
            {
                ReportId = ReplyReportId,
                ReportType = "Receiving",
                SenderId = this.UserId ,
                RecipientId = ReplySender, 
                Content = ReplyMessage,
                Type = "",
                CreatedAt = DateTime.UtcNow

            };
            _context.Messages.Add(msgToTechMem);
            await _context.SaveChangesAsync(); 
            return RedirectToPage(); 

        }
        public IActionResult OnPostAddOrder([FromForm] int ReportId){
            HttpContext.Session.SetInt32("ReportId", ReportId);
            // HttpContext.Session.SetInt32("InboxId", InboxId);


            return RedirectToPage("./ItemCards");
        }
        private void FillLables()
        {
            this.lblRequests = (Program.Translations["Requests"])[Lang];
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
        }
    }
}
