using CalendarQuickstart;
using DataGridViewAutoFilter;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Drive.v3.Data;
using MoreLinq;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using NUnit.Framework.Internal.Execution;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Event = Google.Apis.Calendar.v3.Data.Event;
using DataGridViewAutoFilter;
using Xamarin.Forms;
using System.Net.Mail;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Gmail.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
//using Message = Google.Apis.Gmail.v1.Data.Message;
using MimeKit;
using Message = Google.Apis.Gmail.v1.Data.Message;
using OpenQA.Selenium.Chrome;

namespace InterViewScheduler
{
    public partial class Form1 : Form
    {
        public static Form1 mdiobj;
        public string ApplicationName = "";
        public MailContaint mailContaint = null;
        public GoogleSheetParameters gsp = null;
        private int EditedRowId = 0;
        private string DefaultColorNo = "1";
        private string SchedulerEmail;
        private string InterviewerEmail;
        private string DefaultInterViewerColorCode = "1";
        private int indexRow;
        private BindingSource bindingSource;
        public string Modeof;
        Form2 frm = new Form2();


        CandidateDetails candidateDetails = new CandidateDetails();



        private void Form1_Load(object sender, EventArgs e)
        {
            // ModeOfInterview.SelectedIndex = 0;

        }

        public Form1()
        {
            try
            {
                InitializeComponent();
                Shown += OnShown;
                using (StreamReader r = new StreamReader("Details.json"))
                {
                    string json = r.ReadToEnd();
                    mailContaint = JsonConvert.DeserializeObject<MailContaint>(json);
                    gsp = JsonConvert.DeserializeObject<GoogleSheetParameters>(json);
                    r.Close();
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }

            bindingSource = new BindingSource();
            dgvCandList.DataSource = bindingSource;
        }

        private void OnShown(object sender, EventArgs e)
        {
            txtGoogleMeetUrl.Text = "GoogleMeetLink";
            cmbLocation.DataSource = JsonOperations.ReadLocations().Locations.OrderBy(location => location.Location).ToList();
            cmbLocation.DisplayMember = "Location";
            cmbLocation.ValueMember = "Id";
            cmbInterviewerNames.DataSource = JsonOperations.ReadInterviewers().Interviewers.OrderBy(Interviewer => Interviewer.Name).ToList();
            cmbStatus.DataSource = JsonOperations.ReadFinalStatus().FinalStatus.OrderBy(status => status.Status).ToList();
            cmbSchedulers.DataSource = JsonOperations.ReadSchedulers().Schedulers.OrderBy(Schedulers => Schedulers.Name).ToList();
            cmbRounds.DataSource = JsonOperations.ReadMailTemplates().Template.OrderBy(MailTemplate => MailTemplate.Id).ToList();

            using (StreamReader r = new StreamReader("Details.json"))
            {
                string json = r.ReadToEnd();
                mailContaint = JsonConvert.DeserializeObject<MailContaint>(json);
                gsp = JsonConvert.DeserializeObject<GoogleSheetParameters>(json);
                r.Close();
            }

            GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper(mailContaint.SpreadsheetId);
            //var gsp = new GoogleSheetParameters() { RangeColumnStart, RangeRowStart = 1, RangeColumnEnd = 12, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };

            DataTable dataTable = googleSheetsHelper.ToDataTable(googleSheetsHelper.GetDataFromSheet(gsp));
            //dgvCandList.DataSource = dataTable;




            AddAutoFilterColumn("Id", "Id");
            AddAutoFilterColumn("Name", "Name");
            AddAutoFilterColumn("Mobile", "Mobile");
            AddAutoFilterColumn("Email", "Email");
            AddAutoFilterColumn("Skills", "Skills");
            AddAutoFilterColumn("Location", "Location");
            AddAutoFilterColumn("LastWorkingDate", "LastWorkingDate");
            AddAutoFilterColumn("SchedulersName", "SchedulersName");
            AddAutoFilterColumn("CreatedDate", "CreatedDate");
            AddAutoFilterColumn("InterViewStatus", "InterViewStatus");
            AddAutoFilterColumn("Comment", "Comment");
            AddAutoFilterColumn("InterViewRound", "InterViewRound");
            AddAutoFilterColumn("InterviewDateTime", "InterviewDateTime");
            AddAutoFilterColumn("InterViewerName", "InterViewerName");
            AddAutoFilterColumn("Attendes", "Attendes");
            AddAutoFilterColumn("ResumeLink", "ResumeLink");
            AddAutoFilterColumn("FeedbackLink", "FeedbackLink");
            AddAutoFilterColumn("Duration", "Duration");
            AddAutoFilterColumn("GoogleMeetLink", "GoogleMeetLink");

            bindingSource.DataSource = dataTable.Copy();
        }

        private void AddAutoFilterColumn(string columnName, string dataPropertyName)
        {
            DataGridViewAutoFilterTextBoxColumn column = new DataGridViewAutoFilterTextBoxColumn();
            column.DataPropertyName = dataPropertyName;
            column.HeaderText = columnName;
            dgvCandList.Columns.Add(column);
        }
        private void btnSchedule_Click(object sender, EventArgs e)
        {

            try
            {
                // Set cursor as hourglass
                Modeof = ModeOfInterview.GetItemText(ModeOfInterview.SelectedItem);
                Cursor.Current = Cursors.WaitCursor;

                if (EditedRowId > 0)
                {
                    candidateDetails.Id = EditedRowId;
                    candidateDetails.RecordType = "Update";
                }
                else
                {
                    candidateDetails.Id = (dgvCandList.RowCount <= 0) ? 1 : dgvCandList.RowCount + 1;
                }
                candidateDetails.SchedulersName = ((InterViewScheduler.Schedulers)cmbSchedulers.SelectedItem).Name;
                candidateDetails.SchedulersEmail = ((InterViewScheduler.Schedulers)cmbSchedulers.SelectedItem).Email;
                candidateDetails.Name = txtCondName.Text;
                candidateDetails.Email = txtCandEmail.Text;
                candidateDetails.Mobile = txtCondMobile.Text;
                candidateDetails.Skills = txtSkills.Text;

                //candidateDetails.Mode = ModeOfInterview.GetItemText(ModeOfInterview.SelectedItem);
                candidateDetails.Location = ((InterViewScheduler.Locations)cmbLocation.SelectedItem).Location;
                //CultureInfo culture = new CultureInfo("en-US")
                candidateDetails.LastWorkingDate = dtpLWD.Text;
                candidateDetails.InterViewRound = ((InterViewScheduler.Template)cmbRounds.SelectedItem).Id;
                candidateDetails.InterviewDateTime = dtpInterviewDate.Text + " " + cmbdtpInterviewTime.Text;//Convert.ToDateTime(dtpInterviewDate.Text + " " + cmbdtpInterviewTime.Text, CultureInfo.InvariantCulture);
                candidateDetails.InterViewerName = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).Name;
                candidateDetails.CreatedDate = DateTime.Now.ToShortDateString();
                candidateDetails.Comment = "NA";
                candidateDetails.ResumeLink = txtResumeLink.Text;
                candidateDetails.FeedbackLink = txtFeedbackLink.Text;

                candidateDetails.InterViewerEmail = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).interviewerEmail;
                candidateDetails.ZoomUrl = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).ZoomUrl;
                candidateDetails.PassCode = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).PassCode;
                candidateDetails.MeettingId = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).MeettingId;
                candidateDetails.InterViewStatus = ((InterViewScheduler.FinalStatus)cmbStatus.SelectedItem).Status;
                candidateDetails.GBody = ModeOfInterview.GetItemText(ModeOfInterview.SelectedItem);
                candidateDetails.CandidateDescription = ((InterViewScheduler.Template)cmbRounds.SelectedItem).CandidateDescription;
                candidateDetails.InterViewerDescription = ((InterViewScheduler.Template)cmbRounds.SelectedItem).InterViewerDescription;

                if (candidateDetails.InterViewRound == "1st Round Interview")
                {
                    candidateDetails.Summary = "Invitation for the first round interview with WonderBiz for the " + candidateDetails.Skills;
                }
                else
                {
                    candidateDetails.Summary = "Invitation for the second round interview with WonderBiz for the " + candidateDetails.Skills;
                }

                if (ModeOfInterview.GetItemText(ModeOfInterview.SelectedItem) == "Virtual")
                {
                    candidateDetails.GoogleMeetLink = "GoogleMeetLink";
                    candidateDetails.Mode = "I am thrilled to invite you for a job interview via Google Meet so that we can get to know you better.";
                    candidateDetails.Note = "Note:\r\nTry to log in 10 min prior to the scheduled time.\r\nMake sure you are joining the Google Meet link from the Laptop / Desktop only. It will be required to share your screen.\r\nIt's a video call so make sure your webcam is working and it's ON during the interview.\r\nAlso, make sure Visual Studio / Net Beans / Eclipse is installed on your laptop as it's a technical round. So you will need to do some coding on it.\r\nKindly recheck your Webcam, Mic (Headphones), and Audio (Speaker) of your Laptop / Desktop before you start the interview. If you are logging in from Desktop simultaneously connect from Mobile too for Audio and Video.\r\nIf you get disconnected in between/after 40min kindly rejoin by using the same link.\r\n\r\nIn preparation for your interview, I encourage you to read more about WonderBiz's core values and Benefits. They are the foundation of who we are and how we support each other and our clients. The people you'll be meeting with are looking for candidates who can demonstrate these values. You'll also want to prepare a list of questions for each person you're meeting with to better evaluate if this is a good fit for you.\r\n\r\nVisit Us: www.wonderbizglobal.com\r\nLinkedIn:\r\nhttps://www.linkedin.com/company/wonderbiz-technologies\r\nAlso if you can give Quick Feedback before your Interview:\r\nhttps://forms.gle/AvKx169SDUsxztRE9\r\n\r\nThanks, and good luck with your interview!";
                    candidateDetails.Details = "Below are the details for your video interview. Take a look, and if you have any ques https://accounts.google.com/b/0/AddMailServicetions, don't hesitate to reply to this email.";
                }
                else
                {
                    candidateDetails.Mode = "I am thrilled to invite you for a a F2F discussion so that we can get to know you better.";
                    candidateDetails.Details = "Interview Venue:\r\nWonderBiz Technologies Pvt Ltd.\r\n311/312, Orion Business Park,\r\nNear Kapurbaudi Junction,\r\nGhodbunder Road, \r\nThane-400607";
                    candidateDetails.Note = "Map Link: https://g.page/WonderBiz_Technologies?share\r\nLandmark: Beside Wonder Mall\r\nPlease reply to confirm that this date and time still work for you. \r\nIn preparation for your interview, I encourage you to read more about WonderBiz’s core values and Benefits. They are the foundation of who we are and how we support each other and our clients. The people you’ll be meeting with are looking for candidates who can demonstrate these values. You’ll also want to prepare a list of questions for each person you’re meeting with to better evaluate if this is a good fit for you. \r\nVisit Us: www.wonderbizglobal.com\r\nLinkedIn: https://www.linkedin.com/company/wonderbiz-technologies\r\nAlso if you can give a Quick Feedback before your Interview:\r\nhttps://forms.gle/AvKx169SDUsxztRE9\r\n\r\nThanks, and good luck with your interview!";
                }

                candidateDetails.Attendes = txtAttendes.Text;

                if (!IsAnyNullOrEmpty(candidateDetails))
                {
                    candidateDetails.GoogleMeetLink = txtGoogleMeetUrl.Text;

                    if (cmbDuration.Text != "")
                    {
                        candidateDetails.Duration = Convert.ToInt32(cmbDuration.Text);
                    }
                    else
                    {
                        candidateDetails.Duration = 60;
                    }

                    /*Uplaod files*/

                    if (txtResumeLink.Text != "" && System.IO.File.Exists(txtResumeLink.Text))
                    {
                        var uplaodFileName = UploadFile();
                        candidateDetails.ResumeLink = uplaodFileName;
                    }
                    else
                    {
                        candidateDetails.ResumeLink = txtResumeLink.Text;
                    }
                    /*End Upload Files*/



                    using (StreamReader r = new StreamReader("Details.json"))
                    {
                        string json = r.ReadToEnd();
                        mailContaint = JsonConvert.DeserializeObject<MailContaint>(json);
                        gsp = JsonConvert.DeserializeObject<GoogleSheetParameters>(json);
                        r.Close();
                    }
                    //var gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 12, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };

                    /*Color Code*/
                    candidateDetails.InterViewerColorCode = DefaultInterViewerColorCode;
                    candidateDetails.ColorCode = DefaultColorNo;
                    /*End Color Code*/




                    GoogleCalendarHelper googleCalendarHelper = new GoogleCalendarHelper(mailContaint.SpreadsheetId);

                    Event response = googleCalendarHelper.ScheduleCandidateInterview(candidateDetails);
                    candidateDetails.Summary = txtCondName.Text + " - " + txtSkills.Text + " " + candidateDetails.Location + " @ " + cmbdtpInterviewTime.Text + " " + "'" + Modeof + "'";
                    candidateDetails.ZoomUrl = response.HangoutLink;
                    candidateDetails.GoogleMeetLink = response.HangoutLink;


                    GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper(mailContaint.SpreadsheetId);
                    googleSheetsHelper.AddRow(gsp, candidateDetails);



                    googleCalendarHelper.ScheduleInterViewerInterview(candidateDetails);


                    if (cmbStatus.SelectedValue.ToString() == "Selected")
                    {
                        if (candidateDetails.GoogleMeetLink != "GoogleMeetLink")
                            CopyVideo(candidateDetails.GoogleMeetLink);
                    }

                    ClearData();
                    OnShown(null, null);
                    MessageBox.Show("Data successfully save and Schedule Interview for set date and time.");
                }
                else
                {
                    MessageBox.Show("Please fill all the fields.");
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;
            }

        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void dgvCandList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                indexRow = e.RowIndex;
                DataGridViewRow row = dgvCandList.Rows[indexRow];

                //EditedRowId = row.Cells[0].Value.ToString();
                txtCondName.Text = row.Cells[1].Value.ToString();
                txtCondMobile.Text = row.Cells[2].Value.ToString();
                txtCandEmail.Text = row.Cells[3].Value.ToString();
                txtSkills.Text = row.Cells[4].Value.ToString();
                cmbLocation.Text = row.Cells[5].Value.ToString();
                //dtpLWD.Text = row.Cells[6].Value.ToString();
                cmbSchedulers.Text = row.Cells[7].Value.ToString();

                cmbStatus.Text = row.Cells[9].Value.ToString();
                txtRemark.Text = row.Cells[10].Value.ToString();
                cmbRounds.Text = row.Cells[11].Value.ToString();
                DateTime dt = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[12].Value.ToString(), CultureInfo.InvariantCulture); ;
                String time12 = dt.ToString("h:mm tt", CultureInfo.InvariantCulture);
                dtpInterviewDate.Text = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[12].Value.ToString().Split(' ')[0].ToString(), CultureInfo.InvariantCulture).ToString();
                cmbdtpInterviewTime.Text = time12;
                //dtpInterviewDate.Text = row.Cells[12].Value.ToString();
                cmbInterviewerNames.Text = row.Cells[13].Value.ToString();
                txtAttendes.Text = row.Cells[14].Value.ToString();
                txtResumeLink.Text = row.Cells[15].Value.ToString();
                txtFeedbackLink.Text = row.Cells[16].Value.ToString();
                cmbDuration.Text = row.Cells[17].Value.ToString();
                if (dgvCandList.Rows[e.RowIndex].Cells[18].Value.ToString() == "")
                {
                    txtGoogleMeetUrl.Text = "GoogleMeetLink";
                }
                else
                {
                    txtGoogleMeetUrl.Text = dgvCandList.Rows[e.RowIndex].Cells[18].Value.ToString();
                }
            }
            catch
            {
                // MessageBox.Show("Invalid cell");
            }

        }
        private void dgvCandList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /* try
             {
                 indexRow = e.RowIndex;
                 DataGridViewRow row = dgvCandList.Rows[indexRow];

                 //EditedRowId = row.Cells[0].Value.ToString();
                 txtCondName.Text = row.Cells[1].Value.ToString();
                 txtCondMobile.Text = row.Cells[2].Value.ToString();
                 txtCandEmail.Text = row.Cells[3].Value.ToString();
                 txtSkills.Text = row.Cells[4].Value.ToString();
                 cmbLocation.Text = row.Cells[5].Value.ToString();
                 //dtpLWD.Text = row.Cells[6].Value.ToString();
                 cmbSchedulers.Text = row.Cells[7].Value.ToString();

                 cmbStatus.Text = row.Cells[9].Value.ToString();
                 txtRemark.Text = row.Cells[10].Value.ToString();
                 cmbRounds.Text = row.Cells[11].Value.ToString();
                 DateTime dt = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[12].Value.ToString(), CultureInfo.InvariantCulture); ;
                 String time12 = dt.ToString("h:mm tt", CultureInfo.InvariantCulture);
                 dtpInterviewDate.Text = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[12].Value.ToString().Split(' ')[0].ToString(), CultureInfo.InvariantCulture).ToString();
                 cmbdtpInterviewTime.Text = time12;
                 //dtpInterviewDate.Text = row.Cells[12].Value.ToString();
                 cmbInterviewerNames.Text = row.Cells[13].Value.ToString();
                 txtAttendes.Text = row.Cells[14].Value.ToString();
                 txtResumeLink.Text = row.Cells[15].Value.ToString();
                 txtFeedbackLink.Text = row.Cells[16].Value.ToString();
                 cmbDuration.Text = row.Cells[17].Value.ToString();
                 if (dgvCandList.Rows[e.RowIndex].Cells[18].Value.ToString() == "")
                 {
                     txtGoogleMeetUrl.Text = "GoogleMeetLink";
                 }
                 else
                 {
                     txtGoogleMeetUrl.Text = dgvCandList.Rows[e.RowIndex].Cells[18].Value.ToString();
                 }
             }
             catch
             {
                 // MessageBox.Show("Invalid cell");
             }

 */

        }

        private void ClearData()
        {
            EditedRowId = 0;
            txtCondName.Text = "";
            txtCondMobile.Text = "";
            txtCandEmail.Text = "";
            txtSkills.Text = "";
            dtpLWD.Text = DateTime.Now.ToString();
            txtRemark.Text = "";
            dtpInterviewDate.Text = DateTime.Now.ToString();
            txtAttendes.Text = "";
            txtResumeLink.Text = "";
            txtFeedbackLink.Text = "";
            txtGoogleMeetUrl.Text = "";
            txtGoogleMeetUrl.Text = "GoogleMeetLink";

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CandidateDetails candidateDetails = new CandidateDetails();



            try
            {
                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;

                if (EditedRowId > 0)
                {
                    candidateDetails.Id = EditedRowId;
                    candidateDetails.RecordType = "Update";
                }
                else
                {
                    candidateDetails.Id = (dgvCandList.RowCount <= 0) ? 1 : dgvCandList.RowCount + 1;
                }
                candidateDetails.SchedulersName = ((InterViewScheduler.Schedulers)cmbSchedulers.SelectedItem).Name;
                candidateDetails.SchedulersEmail = ((InterViewScheduler.Schedulers)cmbSchedulers.SelectedItem).Email;
                candidateDetails.Name = txtCondName.Text;
                candidateDetails.Email = txtCandEmail.Text;
                candidateDetails.Mobile = txtCondMobile.Text;

                candidateDetails.FeedbackLink = txtFeedbackLink.Text;

                candidateDetails.Skills = txtSkills.Text;
                candidateDetails.Location = ((InterViewScheduler.Locations)cmbLocation.SelectedItem).Location;
                //CultureInfo culture = new CultureInfo("en-US");
                candidateDetails.LastWorkingDate = dtpLWD.Text;
                candidateDetails.InterViewRound = ((InterViewScheduler.Template)cmbRounds.SelectedItem).Id;
                candidateDetails.InterviewDateTime = dtpInterviewDate.Text + " " + cmbdtpInterviewTime.Text;//Convert.ToDateTime(dtpInterviewDate.Text + " " + cmbdtpInterviewTime.Text, CultureInfo.InvariantCulture);
                candidateDetails.InterViewerName = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).Name;
                candidateDetails.CreatedDate = DateTime.Now.ToShortDateString();
                candidateDetails.Comment = txtRemark.Text;

                candidateDetails.InterViewerEmail = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).interviewerEmail;
                candidateDetails.ZoomUrl = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).ZoomUrl;
                candidateDetails.PassCode = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).PassCode;
                candidateDetails.MeettingId = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).MeettingId;
                candidateDetails.InterViewStatus = ((InterViewScheduler.FinalStatus)cmbStatus.SelectedItem).Status;

                candidateDetails.CandidateDescription = ((InterViewScheduler.Template)cmbRounds.SelectedItem).CandidateDescription;
                candidateDetails.InterViewerDescription = ((InterViewScheduler.Template)cmbRounds.SelectedItem).InterViewerDescription;



                if (candidateDetails.InterViewRound == "1st Round Interview")
                {
                    candidateDetails.Summary = "Invitation for the first round interview with WonderBiz for the " + candidateDetails.Skills;

                }
                else
                {
                    candidateDetails.Summary = "Invitation for the second round interview with WonderBiz for the " + candidateDetails.Skills;
                }

                candidateDetails.Attendes = txtAttendes.Text;

                if (!IsAnyNullOrEmpty(candidateDetails))
                {
                    candidateDetails.GoogleMeetLink = txtGoogleMeetUrl.Text;

                    if (cmbDuration.Text != "")
                    {
                        candidateDetails.Duration = Convert.ToInt32(cmbDuration.Text);
                    }
                    else
                    {
                        candidateDetails.Duration = 60;
                    }

                    /*Uplaod files*/

                    if (txtResumeLink.Text != "" && System.IO.File.Exists(txtResumeLink.Text))
                    {
                        var uplaodFileName = UploadFile();
                        candidateDetails.ResumeLink = uplaodFileName;
                    }
                    else
                    {
                        candidateDetails.ResumeLink = txtResumeLink.Text;
                    }
                    /*End Upload Files*/



                    using (StreamReader r = new StreamReader("Details.json"))
                    {
                        string json = r.ReadToEnd();
                        mailContaint = JsonConvert.DeserializeObject<MailContaint>(json);
                        gsp = JsonConvert.DeserializeObject<GoogleSheetParameters>(json);
                        r.Close();
                    }
                    //var gsp = new GoogleSheetParameters() { RangeColumnStart = 1, RangeRowStart = 1, RangeColumnEnd = 12, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };

                    GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper(mailContaint.SpreadsheetId);
                    googleSheetsHelper.AddRow(gsp, candidateDetails);




                    if (cmbStatus.SelectedValue.ToString() == "Selected")
                    {
                        if (candidateDetails.GoogleMeetLink != "GoogleMeetLink")
                            CopyVideo(candidateDetails.GoogleMeetLink);
                    }

                    ClearData();
                    OnShown(null, null);
                    MessageBox.Show("Data successfully save.");
                }
                else
                {
                    MessageBox.Show("Please fill all the fields.");
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;
            }
        }

        /*  private void btnSearch_Click(object sender, EventArgs e)
          {
              //using (StreamReader r = new StreamReader("Details.json"))
              //{
              //    string json = r.ReadToEnd();
              //    mailContaint = JsonConvert.DeserializeObject<MailContaint>(json);
              //    gsp = JsonConvert.DeserializeObject<GoogleSheetParameters>(json);
              //    r.Close();
              //}
              //GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper(mailContaint.SpreadsheetId);
              ////var gsp = new GoogleSheetParameters() { RangeColumnStart, RangeRowStart = 1, RangeColumnEnd = 12, RangeRowEnd = 100, FirstRowIsHeaders = true, SheetName = "sheet1" };
              //DataTable dt = googleSheetsHelper.ToDataTable(googleSheetsHelper.GetDataFromSheet(gsp));
              //// DataTable fdt = dt.Select("[Name Of Candidate] LIKE '%" + txtSearch.Text + "%' OR [Location] LIKE '%" + txtSearch.Text + "%'").CopyToDataTable();
              //DataTable fdt = new DataTable();
              //foreach (DataRow dr in drs)
              //{
              //    fdt.Rows.Add(dr);
              // }
              //fdt.AcceptChanges();
              //dgvCandList.DataSource = fdt;
              *//*
                          string CadName = Convert.ToString(txtSearch.Text);
                          (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("Name like '%" + CadName + "%'");

                           string CadLocation = Convert.ToString(txtSearch.Text);
                          (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("Location like '%" + CadLocation + "%'");


                          string CadSkill = Convert.ToString(txtSearch.Text);
                          (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("Skills like '%" + CadSkill + "%'");


                          string CadInterviewDateTime = Convert.ToString(txtSearch.Text);
                          (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("InterviewDateTime like '%" + CadInterviewDateTime + "%'");

              *//*



              using (StreamReader r = new StreamReader("Details.json"))
              {
                  string json = r.ReadToEnd();
                  mailContaint = JsonConvert.DeserializeObject<MailContaint>(json);
                  gsp = JsonConvert.DeserializeObject<GoogleSheetParameters>(json);
                  r.Close();
              }

              GoogleSheetsHelper googleSheetsHelper = new GoogleSheetsHelper(mailContaint.SpreadsheetId);

              DataTable dt = googleSheetsHelper.ToDataTable(googleSheetsHelper.GetDataFromSheet(gsp));



              string searchTerm = Convert.ToString(txtSearch.Text);

              DataView dv = (dgvCandList.DataSource as DataTable).DefaultView;

              // Reset the filter before applying a new one
              dv.RowFilter = string.Empty;

              if (!string.IsNullOrEmpty(searchTerm))
              {
                  // Use "OR" to search in either column
                  dv.RowFilter = $"Name like '%{searchTerm}%' OR Location like '%{searchTerm}%' OR Skills like '%{searchTerm}%' OR InterviewDateTime like '%{searchTerm}%' OR SchedulersName like '%{searchTerm}%' OR InterViewStatus like '%{searchTerm}%' OR InterViewRound like '%{searchTerm}%' OR InterViewerName like '%{searchTerm}%'";
              }





          }
  */


        private void btnSingleOpenFileDialog_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Title = "Browse Resume";
            openFileDialog1.DefaultExt = ".docx";
            //openFileDialog1.InitialDirectory = @"%USERPROFILE%\Downloads";

            openFileDialog1.InitialDirectory = GetDownloadsPath();

            openFileDialog1.Filter = "docx files (*.docx)|*.docx|doc files (*.doc)|*.doc|pdf files (*.pdf)|*.pdf";

            //openFileDialog1.ShowDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtResumeLink.Text = openFileDialog1.FileName;

            }
        }
        private static Guid FolderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);
        public static string GetDownloadsPath()
        {
            string path = null;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                IntPtr pathPtr;
                int hr = SHGetKnownFolderPath(ref FolderDownloads, 0, IntPtr.Zero, out pathPtr);
                if (hr == 0)
                {
                    path = Marshal.PtrToStringUni(pathPtr);
                    Marshal.FreeCoTaskMem(pathPtr);
                    return path;
                }
            }
            path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            path = Path.Combine(path, "Downloads");
            return path;
        }

        public string UploadFile()
        {
            GoogleDriveHelper googleDriveHelper = new GoogleDriveHelper();
            string candidateFolderID = "";
            string parentFolderID = "";
            string parentFolderName = "ShdInterview";
            string candidateFolderName = txtCondName.Text + "_" + txtSkills.Text;

            string filePath = txtResumeLink.Text;
            var filename = Path.GetFileName(filePath);

            FileInfo fi = new FileInfo(filePath);

            var newFileName = "resume-" + txtCondName.Text.Trim() + fi.Extension;


            parentFolderID = googleDriveHelper.FolderExists(parentFolderName);
            if (parentFolderID == "")
            {
                parentFolderID = googleDriveHelper.CreateFolder(parentFolderName);
            }

            candidateFolderID = googleDriveHelper.FolderExists(candidateFolderName, parentFolderID);

            if (candidateFolderID == "")
            {
                candidateFolderID = googleDriveHelper.CreateFolder(candidateFolderName, parentFolderID);
            }

            var fileID = googleDriveHelper.getFile(newFileName, candidateFolderID);

            if (fileID != "")
            {
                var a = googleDriveHelper.UploadFile(filePath, newFileName, candidateFolderID, "UPDATE", fileID);

                //googleDriveHelper.getSharedLink(a);
            }
            else
            {
                var a = googleDriveHelper.UploadFile(filePath, newFileName, candidateFolderID);
                //  googleDriveHelper.getSharedLink(a);
            }

            return newFileName;
        }

        public void CopyVideo(string GoogleMeetUrl)
        {


            GoogleDriveHelper googleDriveHelper = new GoogleDriveHelper();

            /*Check for parent folder*/
            string parentFolderName = "ShdInterview";
            var parentFolderID = googleDriveHelper.FolderExists(parentFolderName);
            if (parentFolderID == "")
            {
                parentFolderID = googleDriveHelper.CreateFolder(parentFolderName);
            }

            /*Check candidate folder exists*/
            string candidateFolderName = txtCondName.Text + "_" + txtSkills.Text;
            var candidateFolderID = googleDriveHelper.FolderExists(candidateFolderName, parentFolderID);

            if (candidateFolderID == "")
            {
                candidateFolderID = googleDriveHelper.CreateFolder(candidateFolderName, parentFolderID);
            }

            /*Check if video folder exists*/
            //var  currentVideoFolderID =   googleDriveHelper.FolderExists("Meet Recordings");
            var candidateVideoFileID = googleDriveHelper.getFiles(txtCondName.Text, candidateFolderID);

            /*Delete files*/
            //foreach (var fileId in candidateVideoFileID) {
            //    googleDriveHelper.DeleteFile(fileId);
            //}
            /*Copy files*/
            //CopyFiles(sourceFileID, destinationFolderID);



            /*Candidate video copy and paste*/
            var currentVideoFolderID = googleDriveHelper.FolderExists("Meet Recordings");
            var mainVideoFileID = googleDriveHelper.getFiles(GoogleMeetUrl.Split('/').Last(), currentVideoFolderID);
            foreach (var f in mainVideoFileID)
            {
                //var videFileName = "video-" + f.Value;

                var videFileName = f.Value;

                var isVideoFileExists = googleDriveHelper.getFiles(videFileName, candidateFolderID);

                if (isVideoFileExists.Count == 0)
                {
                    googleDriveHelper.CopyFiles(f.Key, candidateFolderID, videFileName);
                }


            }
        }

        private void cmbSchedulers_SelectedIndexChanged(object sender, EventArgs e)
        {
            DefaultColorNo = ((InterViewScheduler.Schedulers)cmbSchedulers.SelectedItem).ColorCode;

            SchedulerEmail = ((InterViewScheduler.Schedulers)cmbSchedulers.SelectedItem).Email;

            txtAttendes.Text = SchedulerEmail + "," + InterviewerEmail;


            //JsonOperations.ReadSchedulers().Schedulers.OrderBy(Schedulers => Schedulers.Name).ToList();
        }

        private void cmbInterviewerNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            DefaultInterViewerColorCode = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).InterViewerColorCode;

            InterviewerEmail = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).interviewerEmail;

            txtAttendes.Text = SchedulerEmail + "," + InterviewerEmail;
        }

        private void addRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm.parentLocation = this.Location;
            frm.ShowDialog();
        }

        private void addInterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InterviewerAdd interviewerAdd = new InterviewerAdd();
            interviewerAdd.ShowDialog();
        }



        private void btn_Delete_Click(object sender, EventArgs e)
        {
            /*
                        dgvCandList.Rows.RemoveAt(this.dgvCandList.SelectedRows[0].Index);
                        DialogResult result = MessageBox.Show("Do you want to delete this candidate details?", "Delete", MessageBoxButtons.YesNo);
                        MessageBox.Show("Candidate Details Deleted Suceessfully");
            */
            DialogResult result = MessageBox.Show("Do you want to delete this candidate details?", "Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (this.dgvCandList.SelectedRows.Count > 0)
                {
                    dgvCandList.Rows.RemoveAt(this.dgvCandList.SelectedRows[0].Index);
                }
                MessageBox.Show("Candidate Details Deleted Suceessfully");
            }

        }

        private void dtpInterviewDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            DataGridViewAutoFilterColumnHeaderCell.RemoveFilter(dgvCandList);
        }

        private void cmbRounds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRounds.GetItemText(cmbRounds.SelectedItem) == "1st Round Interview")
            {

                txtFeedbackLink.Text = "https://docs.google.com/spreadsheets/d/12GGzryOCG8nC3ErF1zG5G2tlZuCj7cHBOlpmVP1XJts/edit?usp=sharing";

            }
            else
            {
                txtFeedbackLink.Text = "";
            }

        }

        private void AdminToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dgvCandList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCandList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenu.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }




        private void dgvCandList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    dgvCandList.CurrentCell = dgvCandList.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    // Can leave these here - doesn't hurt
                    dgvCandList.Rows[e.RowIndex].Selected = true;
                    dgvCandList.Focus();

                    //selectedBiodataId = Convert.ToInt32(dgrdResults.Rows[e.RowIndex].Cells[1].Value);
                }
                catch (Exception)
                {

                }
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }
        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
        private void selectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                UserCredential credential;

                using (var stream = new FileStream("client_secret_Gmail.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { GmailService.Scope.GmailSend },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                var gmailService = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Your Application Name",
                });
                DataGridViewRow row = dgvCandList.Rows[indexRow];
                if (dgvCandList.SelectedRows.Count == 1)
                {
                    DataGridViewRow selectedRow = dgvCandList.SelectedRows[0];

                    // Get candidate's email and selection status
                    string candidateName = row.Cells[1].Value.ToString();
                    string candidateEmail = row.Cells[3].Value.ToString();
                    string candidateSkill = row.Cells[4].Value.ToString();
                    string candidateLocation = row.Cells[5].Value.ToString();

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Wonderbiz", "suhas.chougule@wonderbiz.in"));
                    emailMessage.To.Add(new MailboxAddress(candidateName, candidateEmail));
                    emailMessage.Subject = "Feedback - " + candidateSkill + "  -  " + candidateLocation;
                    emailMessage.Body = new TextPart("plain")
                    {
                        Text = $"Dear {candidateName},\r\n\r\nWe are thrilled to inform you that you have been selected for the position of .Net SSE at WonderBiz Technology Your skills and experience stood out, and we believe you'll make a valuable addition to our team.\r\n\r\nWill connect you further for the further process.\r\n\r\nVisit Us: www.wonderbiz.co.in\r\nLinkedIn: https://www.linkedin.com/company/wonderbiz-technologies\r\nAlso if you can give a Quick Feedback: https://forms.gle/yWBknaVtbsLy1n9b8"
                    };

                    using (var stream = new MemoryStream())
                    {
                        emailMessage.WriteTo(stream);
                        var rawMessage = Base64UrlEncode(stream.ToArray());

                        var message = new Message
                        {
                            Raw = rawMessage
                        };

                        // Send the email using the Gmail API
                        gmailService.Users.Messages.Send(message, "me").Execute();
                    }

                    MessageBox.Show("Email sent to " + candidateEmail);

                    /*  private string Base64UrlEncode(byte[] input)
                      {
                          return Convert.ToBase64String(input)
                              .Replace('+', '-')
                              .Replace('/', '_')
                              .TrimEnd('=');
                      } */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send email: " + ex.Message);
            }

        }

        private void rejectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                UserCredential credential;

                using (var stream = new FileStream("client_secret_Gmail.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { GmailService.Scope.GmailSend },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                var gmailService = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Your Application Name",
                });
                DataGridViewRow row = dgvCandList.Rows[indexRow];
                if (dgvCandList.SelectedRows.Count == 1)
                {
                    DataGridViewRow selectedRow = dgvCandList.SelectedRows[0];

                    // Get candidate's email and selection status
                    string candidateName = row.Cells[1].Value.ToString();
                    string candidateEmail = row.Cells[3].Value.ToString();
                    string candidateSkill = row.Cells[4].Value.ToString();
                    string candidateLocation = row.Cells[5].Value.ToString();

                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Wonderbiz", "suhas.chougule@wonderbiz.in"));
                    emailMessage.To.Add(new MailboxAddress(candidateName, candidateEmail));
                    emailMessage.Subject = "Feedback - " + candidateSkill + "  -  " + candidateLocation;
                    emailMessage.Body = new TextPart("plain")
                    {
                        Text = $"Dear {candidateName},\r\n\r\nThank you for taking the time to show an interest in the WonderBiz - {candidateSkill} role. It was a pleasure to learn more about your skills and accomplishments.\r\n\r\nUnfortunately, you were not selected to return for additional interviews.\r\nThank you for interviewing with our team.\r\n\r\nI would like to note that often due to high competition for jobs it’s difficult to make choices between many high-caliber candidates. Now that we’ve had the chance to know more about you, we will be keeping your resume for relevant future openings.\r\n\r\nWe wish you success with your current job search. We appreciate your interest in our company.\r\n\r\nVisit Us: www.wonderbiz.co.in\r\nLinkedIn: https://www.linkedin.com/company/wonderbiz-technologies\r\nAlso if you can give a Quick Feedback: https://forms.gle/yWBknaVtbsLy1n9b8"
                    };

                    using (var stream = new MemoryStream())
                    {
                        emailMessage.WriteTo(stream);
                        var rawMessage = Base64UrlEncode(stream.ToArray());

                        var message = new Message
                        {
                            Raw = rawMessage
                        };

                        // Send the email using the Gmail API
                        gmailService.Users.Messages.Send(message, "me").Execute();
                    }

                    MessageBox.Show("Email sent to " + candidateEmail);

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Failed to send email: " + ex.Message);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void OpenGoogleDriveLink(string link)
        {
            try
            {
                // Replace "/view" with "/preview" to get a direct link to the file
               // link = link.Replace("/view", "/preview");

                System.Diagnostics.Process.Start("explorer", link);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening the URL: " + ex.Message);
            }
        }


        private void dgvCandList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 15 || e.ColumnIndex == 16) && e.RowIndex >= 0)
            {
                string link = dgvCandList[e.ColumnIndex, e.RowIndex].Value.ToString();
                if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                {
                    if (link.Contains("https://drive.google.com/"))
                    {
                        OpenGoogleDriveLink(link);
                    }
                    else
                    {
                        System.Diagnostics.Process.Start("explorer", link);
                    }
                }
                else
                {
                    // Handle non-URL data as needed
                    MessageBox.Show("This is not a valid URL: " + link);
                }
            }
        }

       


        private void ModeOfInterview_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
