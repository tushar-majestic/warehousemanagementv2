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

            RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == UserFullName)
            .OrderByDescending(r => r.CreatedAt).ToList();
            // ManagerInboxList = dbContext.ReceivingReports
            //     .Where(r => r.KeeperApproval == true)
            //     .ToList();
             Warehouses = dbContext.Stores.ToList(); 
 
            InboxList = dbContext.Messages
                .Where(s => s.Recipient == UserFullName).OrderByDescending(s => s.CreatedAt).ToList();

            InboxCount = InboxList.Count();
            

            base.ExtractSessionData();
            FillLables();
        }


         public IActionResult OnPostView([FromForm] string ReportId)
        {
                   var dbContext = new LabDBContext();

            HttpContext.Session.SetString("ReportId", ReportId);
            AllRequest = dbContext.ReceivingReports.ToList(); 

            RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == UserFullName).ToList();


            return RedirectToPage("./ViewReceivingReport");
        }

        public IActionResult OnPostAccept([FromForm] int ReceivingReportId)
        {   
           base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            FillLables();
            var dbContext = new LabDBContext();
                AllRequest = dbContext.ReceivingReports.ToList(); 

                RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == UserFullName).ToList();
                this.UserId = HttpContext.Session.GetInt32("UserId");
                InboxList = dbContext.Messages
                .Where(s => s.Recipient == this.UserFullName).ToList();
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
                        report.TechnicalMemberApprovalDate = DateTime.Now;


                        //message to warehouse manager for information
                        string managerMessage = string.Format("Approved the request for items generated by {0}.", report.CreatedBy);
                        var msgToManager = new Message{
                            ReceivingReportId = ReceivingReportId,
                            Sender = this.UserFullName,
                            Recipient = manager.FullName,
                            Content = managerMessage,
                            Type = "Information"

                        };
                        dbContext.Messages.Add(msgToManager);


                        //message to general supervisor for approval
                        //if general Supervisor is assigned.
                        if(generalSup != null){
                            string supMessage = string.Format("Sent Request for Items. Approve the request or add comments.");
                            var msgToSup = new Message{
                                ReceivingReportId = ReceivingReportId,
                                Sender = this.UserFullName,
                                Recipient = generalSup.FullName,
                                Content = supMessage,
                                Type = "Request"
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
                                    ReceivingReportId = ReceivingReportId,
                                    Sender = this.UserFullName,
                                    Recipient = report.CreatedBy,
                                    Content = keeperMessage,
                                    Type = "Accepted"

                            };
                            dbContext.Messages.Add(msgToKeeper);

                        }
                    }
                    else if (this.UserGroupName == "General Supervisor"){
                        //update the general supervisor approval status
                        report.GeneralSupApproval = true;
                        string keeperMessage = string.Format("Your request is accepted by {0}(TechnicalMember) and {1}(General Supervisor).",technicalMember.FullName, generalSup.FullName);
                        var msgToKeeper = new Message{
                                ReceivingReportId = ReceivingReportId,
                                Sender = this.UserFullName,
                                Recipient = report.CreatedBy,
                                Content = keeperMessage,
                                Type = "Accepted"

                        };
                        dbContext.Messages.Add(msgToKeeper);
                    }
                    dbContext.SaveChanges();
                }
                else{
                    Console.WriteLine("report not found");
                }
                return RedirectToPage();
            }
        

        public async Task<IActionResult> OnPostRejectWithComment([FromForm] int RejectReceivingReportId, [FromForm] string Comment)
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
            report.IsRejected = true;
            if(this.UserGroupName == "Technical Member"){
                //If Technical Member reject rhe request than send rejeced message to keeper
                string keeperMessage = string.Format("Your request is rejected with comment: {0}", Comment);
                var msgToKeeper = new Message
                {
                    ReceivingReportId = RejectReceivingReportId,
                    Sender = this.UserFullName ,
                    Recipient = report.CreatedBy, 
                    Content = keeperMessage,
                    Type = "Rejected"
                };
                _context.Messages.Add(msgToKeeper);

            }
            else if(this.UserGroupName == "General Supervisor"){
                //If general supervisor reject the request 

                //Send Informaiton mesage to Manager
                string ManagerMessage = string.Format("Rejected the request for items generated by {0}. with comment: {1}", report.CreatedBy, Comment);
                var msgToManager = new Message
                {
                    ReceivingReportId = RejectReceivingReportId,
                    Sender = this.UserFullName ,
                    Recipient = manager.FullName, 
                    Content = ManagerMessage,
                    Type = "Information"
                };
                _context.Messages.Add(msgToManager);


                //Send message to technical Member
                string TechMemMessage = string.Format("Rejected the request for items generated by {0}. with comment: {1}", report.CreatedBy, Comment);
                var msgToTechMem = new Message
                {
                    ReceivingReportId = RejectReceivingReportId,
                    Sender = this.UserFullName ,
                    Recipient = technicalMember.FullName, 
                    Content = TechMemMessage,
                    Type = "Rejected"
                };
                _context.Messages.Add(msgToTechMem);

           

                
            }
           

            this.UserId= HttpContext.Session.GetInt32("UserId");

            if (this.UserId.HasValue)
            {
                report.RejectedById = this.UserId.Value;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostReply([FromForm] int ReplyReportId, [FromForm] string Reply, [FromForm] string ReplySender ){
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
            string ReplyMessage = string.Format("Replied with comment: {0}", Reply);
            var msgToTechMem = new Message
            {
                ReceivingReportId = ReplyReportId,
                Sender = this.UserFullName ,
                Recipient = ReplySender, 
                Content = ReplyMessage,
                Type = "Reply"
            };
            _context.Messages.Add(msgToTechMem);
            await _context.SaveChangesAsync(); 
            return RedirectToPage(); 

        }
        private void FillLables()
        {
            this.lblRequests = (Program.Translations["Requests"])[Lang];
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
        }
    }
}
