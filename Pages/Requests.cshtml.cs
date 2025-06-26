using Microsoft.AspNetCore.Mvc;
using static LabMaterials.DB.ReturnRequestItem;

namespace LabMaterials.Pages
{
    public class RequestsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        private readonly IWebHostEnvironment _environment;
        public IFormFile AttachmentFile { get; set; }



        public string lblRequests, lblNewReceivingReport, pagetype = "inbox", inboxClass = "btn-dark text-white", outboxClass = "btn-light",lblSearch, lblInbox, lblRequestSent, lblAccept, lblCommentReject, lblReplyResend, lblAddOrderToItemCard, lblDeductOrderItemCard, lblAddRecommendations, lblAttachDestructionReport, lblAddRecyclingNotes, lblAssignSupervisor, lblAssignMembers, lblDeductionDone, lblAccepted,lblFileAttached, lblRejected, lblReplied, lblAddedOrderItemCard, lblAdded, lblCommentRejectRequest, lblYourComment, AcceptSpecifyRecipient, lblAcceptSpecifyRecipient,lblWarehouseKeeper, lblSelectWarehouseKeeper, lblSelectGeneralSpervisor, lblInspectionCommitteeOfficer, lblSelectInspectionCommitteeOfficer, lblDestructionOfficer, lblSelectDestructionOfficer, lblRecyclingOfficer, lblSelectRecyclingOfficer, lblAddAttachment, lblReplyResendRequest, lblYourReply, lblReply;

        public RequestsModel(LabDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public List<ReceivingReport> AllRequest {get; set;}
        public List<ReceivingReport> RequestSent { get; set; }
        public List<MaterialRequest> ManagerRequestSent { get; set; }

        public List<ReturnRequest> SectorManagerRequestSent { get; set; }


        public List<MaterialRequest> AllDispenseRequest { get; set; }

        public List<Message> InboxList { get; set; }

        public IList<Store> Warehouses { get; set; }
        public List<User> AllUsers {get; set;}
        public List<UserGroup> UserGroups {get; set;}

        public string UserFullName;
        public string UserGroupName;
        public int? UserId;
        public int InboxCount;

        public string ReportId;

        public string ErrorMsg { get; set; }
        public List<User> SectorManagerList {  get; set; }
        public List<User> KeeperList {  get; set; }

        public List<User> InspectionOfficerList {  get; set; }

        public List<User> SupervisorList { get; set; }

        public List<User> DestructionOfficerList { get; set; }
        public List<User> RecyclingOfficerList { get; set; }


        public List<Store> Stores { get; set; }




        public void OnGet(string? searchTerm = null)
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

            LoadDropdowns();

            AllRequest = dbContext.ReceivingReports.ToList();
            AllDispenseRequest = dbContext.MaterialRequests.ToList();
            AllUsers = dbContext.Users.ToList();
            UserGroups = dbContext.UserGroups.ToList();
            Warehouses = dbContext.Stores.ToList();


            if (this.UserGroupName == "Warehouse Keeper")
            {
                RequestSent = dbContext.ReceivingReports
                    .Where(r => r.CreatedBy == UserId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                if (!string.IsNullOrEmpty(searchTerm) && pagetype == "outbox")
                {
                    var lowerSearch = searchTerm.ToLower();

                    RequestSent = RequestSent
                        .Where(r =>
                        {
                            var store = Warehouses.FirstOrDefault(u => u.StoreId == int.Parse(r.ReceivingWarehouse));
                            var storename = store?.StoreName?.ToLower() ?? "";

                            return storename.Contains(lowerSearch);

                        }).ToList();


                }
            }
            else if (this.UserGroupName == "Warehouse Manager")
            {
                ManagerRequestSent = dbContext.MaterialRequests
                    .Where(r => r.RequestedByUserId == UserId)
                    .OrderByDescending(r => r.OrderDate)
                    .ToList();

                if (!string.IsNullOrEmpty(searchTerm) && pagetype == "outbox")
                {
                    var lowerSearch = searchTerm.ToLower();
                    ManagerRequestSent = ManagerRequestSent
                        .Where(r =>
                        {
                            var store = Warehouses.FirstOrDefault(u => u.StoreId == r.WarehouseId);
                            var storename = store?.StoreName?.ToLower() ?? "";
                            var DocumentNumber = r.DocumentNumber?.ToLower() ?? "";

                            return storename.Contains(lowerSearch) || DocumentNumber.Contains(lowerSearch);

                        }).ToList();
                }
            }
            else if (this.UserGroupName == "Sector Manager")
            {
                SectorManagerRequestSent = dbContext.ReturnRequests
                    .Where(r => r.CreatedBy == UserId)
                    .OrderByDescending(r => r.OrderDate)
                    .ToList();
                    
                if (!string.IsNullOrEmpty(searchTerm) && pagetype == "outbox")
                {
                    var lowerSearch = searchTerm.ToLower();
                    SectorManagerRequestSent = SectorManagerRequestSent
                        .Where(r =>
                        {
                            var store = Warehouses.FirstOrDefault(u => u.StoreId == r.WarehouseId);
                            var storename = store?.StoreName?.ToLower() ?? "";
                            var OrderNumber = r.OrderNumber?.ToLower() ?? "";

                            return storename.Contains(lowerSearch) || OrderNumber.Contains(lowerSearch);

                        }).ToList();
                }
            }
            
            // ManagerInboxList = dbContext.ReceivingReports
            //     .Where(r => r.KeeperApproval == true)
            //     .ToList();

            // InboxList = dbContext.Messages
            //     .Where(s => s.RecipientId == UserId).OrderByDescending(s => s.CreatedAt).ToList();
            InboxList = dbContext.Messages
            .Where(s => s.RecipientId == UserId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

            if (!string.IsNullOrEmpty(searchTerm) && pagetype == "inbox")
            {
                var lowerSearch = searchTerm.ToLower();

                InboxList = InboxList
                    .Where(m =>
                    {
                        var sender = AllUsers.FirstOrDefault(u => u.UserId == m.SenderId);
                        var senderName = sender?.FullName?.ToLower() ?? "";
                        var messageText = m.Content?.ToLower() ?? "";
                        var ReportType = m.ReportType?.ToLower() ?? "";

                        return senderName.Contains(lowerSearch) || messageText.Contains(lowerSearch) || ReportType.Contains(lowerSearch);
                    })
                    .ToList();
                        
               
            }


            InboxCount = InboxList.Count();


            base.ExtractSessionData();
            FillLables();
        }


        private void LoadDropdowns()
        {
            var dbContext = new LabDBContext();
            //Sector Manager List
            var SecManagerId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Sector Manager")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            SectorManagerList = dbContext.Users
                    .Where(u => u.UserGroupId == SecManagerId)
                    .ToList();
            Stores = dbContext.Stores.ToList();

            //Keeper List
            var KeepId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Warehouse Keeper")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            KeeperList = dbContext.Users
                    .Where(u => u.UserGroupId == KeepId)
                    .ToList();

            //Inspection Committee Officer List
            var InspId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Return Inspection Committee Officer")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            InspectionOfficerList = dbContext.Users
                    .Where(u => u.UserGroupId == InspId)
                    .ToList();

            //General Supervisor list
            var SupervisorId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "General Supervisor")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            SupervisorList = dbContext.Users
                        .Where(u => u.UserGroupId == SupervisorId)
                        .ToList();

            //Destruction Officer List
            var DestOffiId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Destruction Officer")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            DestructionOfficerList = dbContext.Users
                        .Where(u => u.UserGroupId == DestOffiId)
                        .ToList();

            //Recycling Officer List
            var RecylingOffiId = dbContext.UserGroups
                    .Where(g => g.UserGroupName == "Recycling Officer")
                    .Select(g => g.UserGroupId)
                    .FirstOrDefault();

            RecyclingOfficerList = dbContext.Users
                        .Where(u => u.UserGroupId == RecylingOffiId)
                        .ToList();
            
        }

        //  public IActionResult OnPostView([FromForm] int InboxId)
        // {
        //     this.UserGroupName = HttpContext.Session.GetString("UserGroup");

        //     var dbContext = new LabDBContext();

        //         HttpContext.Session.SetString("ReportId", InboxId.ToString());
        //         return RedirectToPage("./ViewReceivingReport");                         

        // }

        public IActionResult OnPostView([FromForm] string ReportType, [FromForm] int? InboxId, [FromForm] int? RequestReportId)
        {
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            var dbContext = new LabDBContext();

            if (ReportType == "Receiving" && InboxId.HasValue)
            {
                HttpContext.Session.SetString("ReportId", InboxId.Value.ToString());
                return RedirectToPage("./ViewReceivingReport");
            }
            else if (ReportType == "Dispensing" && RequestReportId.HasValue)
            {
                HttpContext.Session.SetString("MaterialRequestId", RequestReportId.Value.ToString());
                return RedirectToPage("./ViewDispensedReport");
            }
            else if (ReportType == "ReturnItems" && RequestReportId.HasValue)
            {
                return RedirectToPage("./ReturnRequestsDetails/",  new { id = RequestReportId.Value });
            }

            return RedirectToPage("./Index", new { lang = Lang });
        }

        //Accept and specify recipent in case of dispencing request
        public IActionResult OnPostAcceptAndSpecify([FromForm] int AcceptReportId, [FromForm] int AcceptMessageId, [FromForm] int? ReceipientId)
        {
            //function is used in case when department manager accepts the dispensing request
            base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");

            FillLables();
            var dbContext = new LabDBContext();
            var report = dbContext.MaterialRequests.FirstOrDefault(r => r.RequestId == AcceptReportId);
            if (report != null)
            {
                if (ReceipientId.HasValue)
                {
                    //If Department Manager is logged in than information message is sent to warehouse keeper 
                    if (this.UserGroupName == "Department Manager")
                    {
                        report.DepartmentManagerApproval = true;
                        report.KeeperId = ReceipientId.Value;

                        // report.SectorManagerId = SectorManagerId;
                        report.DeptManagerApprovalDate = DateTime.Now;

                        //message to keeper 
                        string keeperMessage = string.Format("Sent Material Dispensing Request Approve the request or add comments.");
                        var msgToKeeper = new Message
                        {
                            MaterialRequestId = AcceptReportId,
                            ReportType = "Dispensing",
                            SenderId = this.UserId,
                            RecipientId = report.KeeperId,
                            Content = keeperMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToKeeper);



                    }
                    else if (this.UserGroupName == "Warehouse Keeper")
                    {
                        report.KeeperApproval = true;
                        report.KeeperApprovalDate = DateTime.UtcNow;
                        report.SupervisorId = ReceipientId.Value;

                        //message to supervisor
                        string supervisorMessage = string.Format("Sent Material Dispensing Request Approve the request or add comments.");
                        var msgToKeeper = new Message
                        {
                            MaterialRequestId = AcceptReportId,
                            ReportType = "Dispensing",
                            SenderId = this.UserId,
                            RecipientId = report.SupervisorId,
                            Content = supervisorMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToKeeper);


                    }
                }
                else
                {
                    if (this.UserGroupName == "General Supervisor")
                    {
                        report.SupervisorApproval = true;
                        report.SupervisorApprovalDate = DateTime.UtcNow;

                        //message to manager who created the request
                        string managerMessage = string.Format("Has Approved the dispencing request for delivery");
                        var msgToManager = new Message
                        {
                            MaterialRequestId = AcceptReportId,
                            ReportType = "Dispensing",
                            SenderId = this.UserId,
                            RecipientId = report.RequestedByUserId,
                            Content = managerMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToManager);

                        //message to keeper to deduct order from itemcard
                        string keeperMessage = string.Format("Has Approved the dispencing request for delivery");
                        var msgToKeeper = new Message
                        {
                            MaterialRequestId = AcceptReportId,
                            ReportType = "Dispensing",
                            SenderId = this.UserId,
                            RecipientId = report.KeeperId,
                            Content = keeperMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToKeeper);

                        // Below code can be used here in future if deduction is done before

                        // var deductions = dbContext.PendingDeductions
                        // .Where(p => p.MaterialRequestId == AcceptReportId && !p.Status)
                        // .ToList();

                        // foreach (var deduction in deductions)
                        // {
                        //     // 1. Reduce from ItemCard table
                        //     var itemCard = dbContext.ItemCards.FirstOrDefault(i => i.Id == deduction.ItemCardId);
                        //     if (itemCard != null)
                        //     {
                        //         itemCard.QuantityAvailable -= deduction.ReduceQty;
                        //     }

                        //     // 2. Reduce from ItemCardBatches table (adjust logic as needed)
                        //     var batch = dbContext.ItemCardBatches
                        //         .Where(b => b.ItemCardId == deduction.ItemCardId)
                        //         .Where(b => b.RoomId == deduction.RoomId)
                        //         .Where(b => b.ShelfId == deduction.ShelfId)
                        //         .FirstOrDefault();

                        //     if (batch != null && batch.QuantityReceived >= deduction.ReduceQty)
                        //     {
                        //         batch.QuantityReceived -= deduction.ReduceQty;
                        //     }

                        //     // 3. Reduce from ShelveItems table
                        //     var shelveItem = dbContext.ShelveItems
                        //         .FirstOrDefault(s => s.ItemCardId == deduction.ItemCardId &&
                        //                             s.ShelfId == deduction.ShelfId );

                        //     if (shelveItem != null)
                        //     {
                        //         shelveItem.QuantityAvailable -= deduction.ReduceQty;
                        //     }

                        //     // 4. Mark deduction as completed
                        //     deduction.Status = true;
                        // }


                    }
                }

                var message = dbContext.Messages.FirstOrDefault(m => m.Id == AcceptMessageId);

                if (message != null)
                {
                    message.Type = "Accepted";
                }
                dbContext.SaveChanges();

            }

            return RedirectToPage();


        }
        //Destruction Officer will add Destruction Report 

        public async Task<IActionResult> OnPostAddDestructionReportAsync([FromForm] int AcceptReturnMessageId, [FromForm] int AcceptReturnReportId)
        {
            base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");

            FillLables();
            var dbContext = new LabDBContext();
            var Report = dbContext.ReturnRequests.FirstOrDefault(r => r.Id == AcceptReturnReportId);
            if (Report != null)
            {
                if (AttachmentFile != null)
                {
                    Console.WriteLine("Attachment file is done");
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // ensure it exists
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(AttachmentFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AttachmentFile.CopyToAsync(stream);
                    }

                    Report.DestructionReportPath = "/uploads/" + uniqueFileName;
                    Report.DestOffApprovalDate = DateTime.UtcNow;

                    var message = dbContext.Messages.FirstOrDefault(m => m.Id == AcceptReturnMessageId);

                    if (message != null)
                    {
                        message.Type = "File Attached";
                    }
                    //Recycling officer is not assigned only the destruction officer and keeper is assigned than send message to keeper to add order to item card after destruction report is added.
                    if (Report.RecOffId == null)
                    {
                        var CreatedBy = dbContext.Users.FirstOrDefault(u => u.UserId == Report.CreatedBy);

                        string keeperMessage = string.Format("Committee Recommendations were added on the return Items request generated by {0}", CreatedBy.FullName);
                        var keeper = new Message
                        {
                            ReturnRequestId = AcceptReturnReportId,
                            ReportType = "ReturnItems",
                            SenderId = this.UserId,
                            RecipientId = Report.KeeperId,
                            Content = keeperMessage,
                            Type = "Add Order",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(keeper);
                    }
                    dbContext.SaveChanges();

                }
                //Destruction file is required for destruction items
                // else
                // {
                //     //if attachment file is not added
                //     Report.DestOffApprovalDate = DateTime.UtcNow;

                //     var message = dbContext.Messages.FirstOrDefault(m => m.Id == AcceptReturnMessageId);

                //     if (message != null)
                //     {
                //         message.Type = "File Attached";
                //     }
                //     dbContext.SaveChanges();
                // }

            }
            else
            {
                Console.WriteLine("report not found");
            }


            return RedirectToPage();

        }
        

        //Accept and specify recipent in case of Return request
        public IActionResult OnPostAcceptAndSpecifyReturn([FromForm] int AcceptReturnReportId, [FromForm] int AcceptReturnMessageId, [FromForm] int? Receipient, [FromForm] int? destOffId, [FromForm] int? RecyclingOffiId)
        {
            base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");

            FillLables();
            var dbContext = new LabDBContext();
            var report = dbContext.ReturnRequests.FirstOrDefault(r => r.Id == AcceptReturnReportId);
            if (report != null)
            {
                if (Receipient.HasValue)
                {
                    //If Warehouse Manager is logged in than send message to Return Inspection Comittee Officer
                    if (this.UserGroupName == "Warehouse Manager")
                    {
                        report.InspOffId = Receipient.Value;

                        report.ManagerApprovalDate = DateTime.UtcNow;

                        //message to Inspection Officer
                        string InspOffMessage = string.Format("Sent Return Items Request add Committee recommendations.");
                        var msgToInspOfficer = new Message
                        {
                            ReturnRequestId = AcceptReturnReportId,
                            ReportType = "ReturnItems",
                            SenderId = this.UserId,
                            RecipientId = report.InspOffId,
                            Content = InspOffMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToInspOfficer);



                    }
                    //If Return Inspection Committee Officer is logged in than send message to Supervisor
                    else if (this.UserGroupName == "Return Inspection Committee Officer")
                    {
                        report.SupervisorId = Receipient.Value;
                        report.InspOffApprovalDate = DateTime.UtcNow;

                        //message to Supervisor 
                        string SupervisorMessage = string.Format("Sent Return Items Request Approve the request or add comments.");
                        var msgToSupervisor = new Message
                        {
                            ReturnRequestId = AcceptReturnReportId,
                            ReportType = "ReturnItems",
                            SenderId = this.UserId,
                            RecipientId = report.SupervisorId,
                            Content = SupervisorMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToSupervisor);

                    }
                    else if (this.UserGroupName == "General Supervisor")
                    {
                        report.DestOffId = destOffId.HasValue ? destOffId.Value : null;
                        report.KeeperId = Receipient.Value;
                        report.RecOffId = RecyclingOffiId.HasValue ? RecyclingOffiId.Value : null;
                        report.SupervisorApprovalDate = DateTime.UtcNow;

                        //message to Destruction Officer 
                        if (destOffId.HasValue)
                        {
                            string DestOfficerMessage = string.Format("Sent Return Items Request Attach Destruction report.");
                            var DestOfficer = new Message
                            {
                                ReturnRequestId = AcceptReturnReportId,
                                ReportType = "ReturnItems",
                                SenderId = this.UserId,
                                RecipientId = report.DestOffId,
                                Content = DestOfficerMessage,
                                Type = "",
                                CreatedAt = DateTime.UtcNow
                            };
                            dbContext.Messages.Add(DestOfficer);
                        }
                        

                        var CreatedBy = dbContext.Users.FirstOrDefault(u => u.UserId == report.CreatedBy);

                        //message to keeper  if destrutcion officer is
                        if (destOffId.HasValue || RecyclingOffiId.HasValue)
                        {
                            string keeperMessage = string.Format("Committee Recommendations were added on the return Items request generated by {0}", CreatedBy.FullName);
                            var keeper = new Message
                            {
                                ReturnRequestId = AcceptReturnReportId,
                                ReportType = "ReturnItems",
                                SenderId = this.UserId,
                                RecipientId = report.KeeperId,
                                Content = keeperMessage,
                                Type = "",
                                CreatedAt = DateTime.UtcNow
                            };
                            dbContext.Messages.Add(keeper);
                        }
                        else if (!destOffId.HasValue && !RecyclingOffiId.HasValue)
                        {
                            string keeperMessage = string.Format("Committee Recommendations were added on the return Items request generated by {0}", CreatedBy.FullName);
                            var keeper = new Message
                            {
                                ReturnRequestId = AcceptReturnReportId,
                                ReportType = "ReturnItems",
                                SenderId = this.UserId,
                                RecipientId = report.KeeperId,
                                Content = keeperMessage,
                                Type = "Add Order",
                                CreatedAt = DateTime.UtcNow
                            };
                            dbContext.Messages.Add(keeper);
                        }

                        
                     

                        //message to recycling Officer 
                        if (RecyclingOffiId.HasValue)
                        {
                            string recyclingOfficerMsg = string.Format("Add recycling notes to specific recycling Items in the Return Items Request generated by {0}", CreatedBy.FullName);
                            var recyclingOfficer = new Message
                            {
                                ReturnRequestId = AcceptReturnReportId,
                                ReportType = "ReturnItems",
                                SenderId = this.UserId,
                                RecipientId = report.RecOffId,
                                Content = recyclingOfficerMsg,
                                Type = "",
                                CreatedAt = DateTime.UtcNow
                            };
                            dbContext.Messages.Add(recyclingOfficer);
                        }

                    }

                }

                var message = dbContext.Messages.FirstOrDefault(m => m.Id == AcceptReturnMessageId);

                if (message != null)
                {
                    message.Type = "Accepted";
                }
                dbContext.SaveChanges();

            }
            // if (!ModelState.IsValid)
            // {
            //     return Page();
            // }
            return RedirectToPage();


        }
        

        public IActionResult OnPostEditReturnRequest([FromForm] int ReturnRequestId, [FromForm] int InboxId)
        {



            HttpContext.Session.SetInt32("ReturnRequestId", ReturnRequestId);
            HttpContext.Session.SetInt32("InboxId", InboxId);

            return RedirectToPage("./EditReturnRequest");




        }


        public IActionResult OnPostAcceptDispencing([FromForm] int AcceptReportId, [FromForm] int AcceptMessageId)
        {
            base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            FillLables();
            var dbContext = new LabDBContext();
            AllRequest = dbContext.ReceivingReports.ToList();
            AllDispenseRequest = dbContext.MaterialRequests.ToList();

            this.UserId = HttpContext.Session.GetInt32("UserId");

            if (this.UserGroupName == "Warehouse Keeper")
                RequestSent = dbContext.ReceivingReports.Where(r => r.CreatedBy == this.UserId).ToList();

            InboxList = dbContext.Messages
            .Where(s => s.RecipientId == this.UserId).ToList();
            var report = dbContext.MaterialRequests.FirstOrDefault(r => r.RequestId == AcceptReportId);

            if (report != null)
            {

                //if Warehouuse Keeper accepts the request message is sent to sector manager
                if (this.UserGroupName == "Warehouse Keeper")
                {
                    report.KeeperApproval = true;
                    report.KeeperApprovalDate = DateTime.UtcNow;

                    //message to Receipient (sector manager)
                    string sectorManagerMessage = string.Format("Sent Material Dispensing Request Approve the request or add comments.");
                    var msgToSectorManager = new Message
                    {
                        MaterialRequestId = AcceptReportId,
                        ReportType = "Dispensing",
                        SenderId = this.UserId,
                        RecipientId = report.SupervisorId,
                        Content = sectorManagerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow
                    };
                    dbContext.Messages.Add(msgToSectorManager);


                }


                var message = dbContext.Messages.FirstOrDefault(m => m.Id == AcceptMessageId);

                if (message != null)
                {
                    message.Type = "Accepted";
                }
                dbContext.SaveChanges();

            }
            else
            {
                Console.WriteLine("report not found");
            }
            return RedirectToPage();
        }
        
        public IActionResult OnPostAccept([FromForm] int ReceivingReportId, [FromForm] int MessageId)
        {
            base.ExtractSessionData();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            FillLables();
            var dbContext = new LabDBContext();
            AllRequest = dbContext.ReceivingReports.ToList();
            AllDispenseRequest = dbContext.MaterialRequests.ToList();

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
                if (this.UserGroupName == "Technical Member")
                {
                    var manager = dbContext.Users.FirstOrDefault(u => u.UserId == report.RecipientEmployeeId);
                    //update the technical member approval status
                    report.TechnicalMemberApproval = true;
                    report.IsRejectedByTechnicalMember = false;
                    report.TechnicalMemberApprovalDate = DateTime.Now;

                    if (manager != null)
                    {
                        var CreatedBy = dbContext.Users.FirstOrDefault(u => u.UserId == report.CreatedBy);

                        //message to warehouse manager for information
                        string managerMessage = string.Format("Approved the request for items generated by {0}.", CreatedBy.FullName);
                        var msgToManager = new Message
                        {
                            ReportId = ReceivingReportId,
                            ReportType = "Receiving",
                            SenderId = this.UserId,
                            RecipientId = report.RecipientEmployeeId,
                            Content = managerMessage,
                            Type = "",
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.Messages.Add(msgToManager);
                    }




                    //message to general supervisor for approval
                    //if general Supervisor is assigned.
                    if (generalSup != null)
                    {
                        string supMessage = string.Format("Sent Request for Items. Approve the request or add comments.");
                        var msgToSup = new Message
                        {
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
                    else
                    {
                        //assume it is is approved and set generalsupapproval to true
                        report.GeneralSupApproval = true;
                        report.GeneralSupervisorApprovalDate = DateTime.Now;

                        //send message to keeper
                        string keeperMessage = string.Format("Your request is accepted by {0}(TechnicalMember) and no general supervisor was added.", technicalMember.FullName);
                        var msgToKeeper = new Message
                        {
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
                else if (this.UserGroupName == "General Supervisor")
                {
                    //update the general supervisor approval status
                    report.GeneralSupApproval = true;
                    report.IsRejectedByGeneralSupervisor = false;
                    report.GeneralSupervisorApprovalDate = DateTime.Now;


                    string keeperMessage = string.Format("Your request is accepted by {0}(TechnicalMember) and {1}(General Supervisor).", technicalMember.FullName, generalSup.FullName);
                    var msgToKeeper = new Message
                    {
                        ReportId = ReceivingReportId,
                        ReportType = "Receiving",
                        SenderId = this.UserId,
                        RecipientId = report.CreatedBy,
                        Content = keeperMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow


                    };
                    dbContext.Messages.Add(msgToKeeper);

                    var CreatedBy = dbContext.Users.FirstOrDefault(u => u.UserId == report.CreatedBy);

                    //message to warehouse manager for information
                    string managerMessage = string.Format("Approved the request for items generated by {0}.", CreatedBy.FullName);
                    var msgToManager = new Message
                    {
                        ReportId = ReceivingReportId,
                        ReportType = "Receiving",
                        SenderId = this.UserId,
                        RecipientId = report.RecipientEmployeeId,
                        Content = managerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow
                    };
                    dbContext.Messages.Add(msgToManager);
                }
                var message = dbContext.Messages.FirstOrDefault(m => m.Id == MessageId);

                if (message != null)
                {
                    message.Type = "Accepted";
                }
                dbContext.SaveChanges();

            }
            else
            {
                Console.WriteLine("report not found");
            }
            return RedirectToPage();
        }
        

        public async Task<IActionResult> OnPostRejectWithComment([FromForm] int RejectReceivingReportId, [FromForm] string Comment, [FromForm] int RejectMessageId, [FromForm] string ReportType)
        {  
            base.ExtractSessionData();
            FillLables();
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");



            if (ReportType == "Dispensing")
            {
                var report = await _context.MaterialRequests.FindAsync(RejectReceivingReportId);
                if (report == null)
                {
                    // handle error
                    return NotFound();
                }
                if (this.UserGroupName == "Department Manager")
                {
                    if (report.DepartmentManagerApproval == true)
                        report.DepartmentManagerApproval = false;

                    string managerMessage = string.Format("Your request is rejected with comment: {0}", Comment);
                    var msgToManager = new Message
                    {
                        MaterialRequestId = RejectReceivingReportId,
                        ReportType = "Dispensing",
                        SenderId = this.UserId,
                        RecipientId = report.RequestedByUserId,
                        Content = managerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToManager);
                }
                else if (this.UserGroupName == "Warehouse Keeper")
                {
                    if (report.KeeperApproval == true)
                        report.KeeperApproval = false;

                    string deptManagerMessage = string.Format("Your request is rejected with comment: {0}", Comment);
                    var msgToManager = new Message
                    {
                        MaterialRequestId = RejectReceivingReportId,
                        ReportType = "Dispensing",
                        SenderId = this.UserId,
                        RecipientId = report.DeptManagerId,
                        Content = deptManagerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToManager);
                }
                else if (this.UserGroupName == "General Supervisor")
                {
                    if (report.SupervisorApproval == true)
                        report.SupervisorApproval = false;

                    string keeperMessage = string.Format("Your request is rejected with comment: {0}", Comment);
                    var msgToKeeper = new Message
                    {
                        MaterialRequestId = RejectReceivingReportId,
                        ReportType = "Dispensing",
                        SenderId = this.UserId,
                        RecipientId = report.KeeperId,
                        Content = keeperMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToKeeper);
                }

            }
            else if (ReportType == "Receiving")
            {

                var report = await _context.ReceivingReports.FindAsync(RejectReceivingReportId);
                if (report == null)
                {
                    // handle error
                    return NotFound();
                }
                var generalSup = dbContext.Users.FirstOrDefault(u => u.UserId == report.ChiefResponsibleId);

                var technicalMember = dbContext.Users.FirstOrDefault(u => u.UserId == report.TechnicalMemberId);
                var manager = dbContext.Users.FirstOrDefault(u => u.UserId == report.RecipientEmployeeId);
                if (this.UserGroupName == "Technical Member")
                {
                    //If Technical Member reject rhe request than send rejeced message to keeper
                    report.IsRejectedByTechnicalMember = true;
                    if (report.TechnicalMemberApproval == true)
                        report.TechnicalMemberApproval = false;

                    string keeperMessage = string.Format("Your request is rejected with comment: {0}", Comment);
                    var msgToKeeper = new Message
                    {
                        ReportId = RejectReceivingReportId,
                        ReportType = "Receiving",
                        SenderId = this.UserId,
                        RecipientId = report.CreatedBy,
                        Content = keeperMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToKeeper);

                }
                else if (this.UserGroupName == "General Supervisor")
                {
                    //If general supervisor reject the request 

                    //Send Informaiton mesage to Manager
                    report.IsRejectedByGeneralSupervisor = true;
                    if (report.GeneralSupApproval == true)
                        report.GeneralSupApproval = false;

                    var CreatedBy = dbContext.Users.FirstOrDefault(u => u.UserId == report.CreatedBy);
                    string ManagerMessage = string.Format("Rejected the request for items generated by {0}. with comment: {1}", CreatedBy.FullName, Comment);
                    var msgToManager = new Message
                    {
                        ReportId = RejectReceivingReportId,
                        ReportType = "Receiving",
                        SenderId = this.UserId,
                        RecipientId = report.RecipientEmployeeId,
                        Content = ManagerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow

                    };
                    _context.Messages.Add(msgToManager);


                    //Send message to technical Member
                    string TechMemMessage = string.Format("Rejected the request for items generated by {0}. with comment: {1}", CreatedBy.FullName, Comment);
                    var msgToTechMem = new Message
                    {
                        ReportId = RejectReceivingReportId,
                        ReportType = "Receiving",
                        SenderId = this.UserId,
                        RecipientId = report.TechnicalMemberId,
                        Content = TechMemMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow

                    };
                    _context.Messages.Add(msgToTechMem);

                }

            }
            else if (ReportType == "ReturnItems")
            {
                var report = await _context.ReturnRequests.FindAsync(RejectReceivingReportId);
                if (report == null)
                {
                    return NotFound();
                }
                if (this.UserGroupName == "Warehouse Manager")
                {
                    if (report.ManagerApprovalDate != null)
                        report.ManagerApprovalDate = null;

                    string SectorManagerMessage = string.Format("Your return items request is rejected with comment: {0}", Comment);
                    var msgToSectorManager = new Message
                    {
                        ReturnRequestId = RejectReceivingReportId,
                        ReportType = "ReturnItems",
                        SenderId = this.UserId,
                        RecipientId = report.CreatedBy,
                        Content = SectorManagerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToSectorManager);
                }
                else if (this.UserGroupName == "General Supervisor")
                {
                    if (report.SupervisorApprovalDate != null)
                        report.SupervisorApprovalDate = null;

                    string InspOffiMessage = string.Format("Your return items request is rejected with comment: {0}", Comment);
                    var msgToInspOffi = new Message
                    {
                        ReturnRequestId = RejectReceivingReportId,
                        ReportType = "ReturnItems",
                        SenderId = this.UserId,
                        RecipientId = report.InspOffId,
                        Content = InspOffiMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToInspOffi);
                }
                else if (this.UserGroupName == "Return Inspection Committee Officer")
                {
                    if (report.InspOffApprovalDate != null)
                        report.InspOffApprovalDate = null;
                        
                    string ManagerMessage = string.Format("Your return items request is rejected with comment: {0}", Comment);
                    var msgToManager = new Message
                    {
                        ReturnRequestId = RejectReceivingReportId,
                        ReportType = "ReturnItems",
                        SenderId = this.UserId,
                        RecipientId = report.ManagerId,
                        Content = ManagerMessage,
                        Type = "",
                        CreatedAt = DateTime.UtcNow,


                    };
                    _context.Messages.Add(msgToManager);
                }

            }
            this.UserId= HttpContext.Session.GetInt32("UserId");

            // if (this.UserId.HasValue)
            // {
            //     report.RejectedById = this.UserId.Value;
            // }
            var message = dbContext.Messages.FirstOrDefault(m => m.Id == RejectMessageId);

            if (message != null)
            {
                message.Type = "Rejected"; 
            }
            dbContext.SaveChanges();

           

            await _context.SaveChangesAsync();

            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnPostReply([FromForm] int ReplyReportId, [FromForm] string Reply, [FromForm] int? ReplySender, [FromForm] int ReplyMessageId, [FromForm] string ReplyReportType ){
            base.ExtractSessionData();
            FillLables();
            Console.WriteLine("reply function");
            var dbContext = new LabDBContext();
            this.UserFullName = HttpContext.Session.GetString("FullName");
            this.UserGroupName = HttpContext.Session.GetString("UserGroup");
            this.UserId = HttpContext.Session.GetInt32("UserId");

            if (ReplyReportType == "Dispensing")
            {
                var report = await _context.MaterialRequests.FindAsync(ReplyReportId);
                if (report == null)
                {
                    return NotFound();
                }

                string ReplyMessage = string.Format("Replied with comment: {0}", Reply);
                var msgTo = new Message
                {
                    MaterialRequestId = ReplyReportId,
                    ReportType = "Dispensing",
                    SenderId = this.UserId,
                    RecipientId = ReplySender,
                    Content = ReplyMessage,
                    Type = "",
                    CreatedAt = DateTime.UtcNow

                };
                _context.Messages.Add(msgTo);

            }
            else if (ReplyReportType == "Receiving")
            {

                var report = await _context.ReceivingReports.FindAsync(ReplyReportId);
                if (report == null)
                {
                    return NotFound();

                }

                // report.IsReplied = true;

                string ReplyMessage = string.Format("Replied with comment: {0}", Reply);
                var msgToTechMem = new Message
                {
                    ReportId = ReplyReportId,
                    ReportType = "Receiving",
                    SenderId = this.UserId,
                    RecipientId = ReplySender,
                    Content = ReplyMessage,
                    Type = "",
                    CreatedAt = DateTime.UtcNow

                };
                _context.Messages.Add(msgToTechMem);
            }
            else if (ReplyReportType == "ReturnItems")
            {
                var report = await _context.ReturnRequests.FindAsync(ReplyReportId);
                if (report == null)
                {
                    return NotFound();

                }

                // report.IsReplied = true;

                string ReplyMessage = string.Format("Replied with comment: {0}", Reply);
                var msgToSender = new Message
                {
                    ReturnRequestId = ReplyReportId,
                    ReportType = "ReturnItems",
                    SenderId = this.UserId,
                    RecipientId = ReplySender,
                    Content = ReplyMessage,
                    Type = "",
                    CreatedAt = DateTime.UtcNow

                };
                _context.Messages.Add(msgToSender);
            }

            var message = dbContext.Messages.FirstOrDefault(m => m.Id == ReplyMessageId);
            if (message != null)
            {
                message.Type = "Replied"; 
            }
            dbContext.SaveChanges();

            await _context.SaveChangesAsync(); 
            return RedirectToPage(); 

        }
        public IActionResult OnPostAddOrder([FromForm] int ReportId, [FromForm] int InboxId, [FromForm] string ReportType){
            HttpContext.Session.SetInt32("ReportId", ReportId);
              HttpContext.Session.SetInt32("InboxId", InboxId);
            HttpContext.Session.SetString("ReportType", ReportType);



            return RedirectToPage("./ItemCards");
        }

        public IActionResult OnPostDeductOrder([FromForm] int DisReportId, [FromForm] int MsgId)
        {
            HttpContext.Session.SetInt32("DisReportId", DisReportId);
            HttpContext.Session.SetInt32("MsgId", MsgId);


            return RedirectToPage("./DeductOrder");
        }

        public JsonResult OnGetHasRecyclingItems(int returnRequestId)
        {
            var dbContext = new LabDBContext();

           

            var hasRecyclable = dbContext.ReturnRequestItems
                .Any(r => r.ReturnRequestId == returnRequestId && r.RecommendedAction == ItemCondition.Recyclable);
            var hasDestruction = dbContext.ReturnRequestItems
                .Any(r => r.ReturnRequestId == returnRequestId && r.RecommendedAction == ItemCondition.Destruction);

            return new JsonResult(new { hasRecyclable, hasDestruction });
        } 

        private void FillLables()
        {
            this.lblRequests = (Program.Translations["Requests"])[Lang];
            this.lblNewReceivingReport = (Program.Translations["NewReceivingReport"])[Lang];
            this.lblSearch = (Program.Translations["Search"])[Lang];
            this.lblInbox = (Program.Translations["Inbox"])[Lang];
            this.lblRequestSent = (Program.Translations["RequestSent"])[Lang];
            this.lblAccept = (Program.Translations["Accept"])[Lang];
            this.lblCommentReject = (Program.Translations["CommentReject"])[Lang];
            this.lblReplyResend = (Program.Translations["ReplyResend"])[Lang];
            this.lblAddOrderToItemCard = (Program.Translations["AddOrderToItemCard"])[Lang];
            this.lblDeductOrderItemCard = (Program.Translations["DeductOrderItemCard"])[Lang];
            this.lblAddRecommendations = (Program.Translations["AddRecommendations"])[Lang];
            this.lblAttachDestructionReport = (Program.Translations["AttachDestructionReport"])[Lang];
            this.lblAddRecyclingNotes = (Program.Translations["AddRecyclingNotes"])[Lang];
            this.lblAssignSupervisor = (Program.Translations["AssignSupervisor"])[Lang];
            this.lblAssignMembers = (Program.Translations["AssignMembers"])[Lang];
            this.lblDeductionDone = (Program.Translations["DeductionDone"])[Lang];
            this.lblAccepted = (Program.Translations["Accepted"])[Lang];
            this.lblFileAttached = (Program.Translations["FileAttached"])[Lang];
            this.lblRejected = (Program.Translations["Rejected"])[Lang];
            this.lblReplied = (Program.Translations["Replied"])[Lang];
            this.lblAddedOrderItemCard = (Program.Translations["AddedOrderItemCard"])[Lang];
            this.lblAdded = (Program.Translations["Added"])[Lang];
            this.lblCommentRejectRequest = (Program.Translations["CommentRejectRequest"])[Lang];
            this.lblYourComment = (Program.Translations["YourComment"])[Lang];
            this.lblAcceptSpecifyRecipient = (Program.Translations["AcceptSpecifyRecipient"])[Lang];
            this.lblWarehouseKeeper = (Program.Translations["WarehouseKeeper"])[Lang];
            this.lblSelectWarehouseKeeper = (Program.Translations["SelectWarehouseKeeper"])[Lang];
            this.lblSelectGeneralSpervisor = (Program.Translations["SelectGeneralSpervisor"])[Lang];
            this.lblInspectionCommitteeOfficer = (Program.Translations["InspectionCommitteeOfficer"])[Lang];
            this.lblSelectInspectionCommitteeOfficer = (Program.Translations["SelectInspectionCommitteeOfficer"])[Lang];
            this.lblDestructionOfficer = (Program.Translations["DestructionOfficer"])[Lang];
            this.lblSelectDestructionOfficer = (Program.Translations["SelectDestructionOfficer"])[Lang];
            this.lblRecyclingOfficer = (Program.Translations["RecyclingOfficer"])[Lang];
            this.lblSelectRecyclingOfficer = (Program.Translations["SelectRecyclingOfficer"])[Lang];
            this.lblAddAttachment = (Program.Translations["AddAttachment"])[Lang];
            this.lblReplyResendRequest = (Program.Translations["ReplyResendRequest"])[Lang];
            this.lblYourReply = (Program.Translations["YourReply"])[Lang];

            this.lblReply = (Program.Translations["Reply"])[Lang];


        }
    }
}
