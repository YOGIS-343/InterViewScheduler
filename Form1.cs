using CalendarQuickstart;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Drive.v3.Data;
//using MongoDB.Bson.IO;
using Newtonsoft.Json;
using NUnit.Framework.Internal.Execution;
using System.Data;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Event = Google.Apis.Calendar.v3.Data.Event;

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
        private string DefaultInterViewerColorCode = "1";
        private int indexRow;

        CandidateDetails candidateDetails = new CandidateDetails();
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
            dgvCandList.DataSource = dataTable;

        }

        private void btnSchedule_Click(object sender, EventArgs e)
        {

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
                candidateDetails.Skills = txtSkills.Text;
                candidateDetails.Location = ((InterViewScheduler.Locations)cmbLocation.SelectedItem).Location;
                //CultureInfo culture = new CultureInfo("en-US");
                candidateDetails.LastWorkingDate = dtpLWD.Text;
                candidateDetails.InterViewRound = ((InterViewScheduler.Template)cmbRounds.SelectedItem).Id;
                candidateDetails.InterviewDateTime = dtpInterviewDate.Text + " " + cmbdtpInterviewTime.Text;//Convert.ToDateTime(dtpInterviewDate.Text + " " + cmbdtpInterviewTime.Text, CultureInfo.InvariantCulture);
                candidateDetails.InterViewerName = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).Name;
                candidateDetails.CreatedDate = DateTime.Now.ToShortDateString();
                candidateDetails.Comment = txtRemark.Text;
                candidateDetails.ResumeLink = txtResumeLink.Text;
                candidateDetails.FeedbackLink = txtFeedbackLink.Text;

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

                    /*Color Code*/
                    candidateDetails.InterViewerColorCode = DefaultInterViewerColorCode;
                    candidateDetails.ColorCode = DefaultColorNo;
                    /*End Color Code*/




                    GoogleCalendarHelper googleCalendarHelper = new GoogleCalendarHelper(mailContaint.SpreadsheetId);

                    Event response = googleCalendarHelper.ScheduleCandidateInterview(candidateDetails);
                    candidateDetails.Summary = txtCondName.Text + " - " + txtSkills.Text + " " + candidateDetails.Location + " @ " + cmbdtpInterviewTime.Text;
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
            if (e.RowIndex >= 0)
            {
                var dataIndexNo = dgvCandList.Rows[e.RowIndex].Index.ToString();
                EditedRowId = Convert.ToInt32(dgvCandList.Rows[e.RowIndex].Cells[0].Value.ToString());
                txtCondName.Text = dgvCandList.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtCondMobile.Text = dgvCandList.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtCandEmail.Text = dgvCandList.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtSkills.Text = dgvCandList.Rows[e.RowIndex].Cells[4].Value.ToString();
                cmbLocation.SelectedItem = (cmbLocation.DataSource as List<Locations>).First(x => x.Location.Equals(dgvCandList.Rows[e.RowIndex].Cells[5].Value.ToString()));
                dtpLWD.Text = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[6].Value.ToString(), CultureInfo.InvariantCulture).ToString();
                cmbSchedulers.SelectedItem = (cmbSchedulers.DataSource as List<Schedulers>).First(x => x.Name.Equals(dgvCandList.Rows[e.RowIndex].Cells[7].Value.ToString()));
                cmbStatus.SelectedItem = (cmbStatus.DataSource as List<FinalStatus>).First(x => x.Status.Equals(dgvCandList.Rows[e.RowIndex].Cells[9].Value.ToString()));
                txtRemark.Text = dgvCandList.Rows[e.RowIndex].Cells[10].Value.ToString();
                cmbRounds.SelectedItem = (cmbRounds.DataSource as List<Template>).First(x => x.Id.Equals(dgvCandList.Rows[e.RowIndex].Cells[11].Value.ToString()));
                DateTime dt = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[12].Value.ToString(), CultureInfo.InvariantCulture); ;
                String time12 = dt.ToString("h:mm tt", CultureInfo.InvariantCulture);
                dtpInterviewDate.Text = Convert.ToDateTime(dgvCandList.Rows[e.RowIndex].Cells[12].Value.ToString().Split(' ')[0].ToString(), CultureInfo.InvariantCulture).ToString();
                cmbdtpInterviewTime.Text = time12;
                cmbInterviewerNames.SelectedItem = (cmbInterviewerNames.DataSource as List<Interviewers>).First(x => x.Name.Equals(dgvCandList.Rows[e.RowIndex].Cells[13].Value.ToString()));
                txtAttendes.Text = dgvCandList.Rows[e.RowIndex].Cells[14].Value.ToString();
                txtResumeLink.Text = dgvCandList.Rows[e.RowIndex].Cells[15].Value.ToString();
                txtFeedbackLink.Text = dgvCandList.Rows[e.RowIndex].Cells[16].Value.ToString();
                cmbDuration.Text = dgvCandList.Rows[e.RowIndex].Cells[17].Value.ToString();
                if (dgvCandList.Rows[e.RowIndex].Cells[18].Value.ToString() == "")
                {
                    txtGoogleMeetUrl.Text = "GoogleMeetLink";
                }
                else
                {
                    txtGoogleMeetUrl.Text = dgvCandList.Rows[e.RowIndex].Cells[18].Value.ToString();
                }

            }
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

        private void btnSearch_Click(object sender, EventArgs e)
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
            /*
                        string CadName = Convert.ToString(txtSearch.Text);
                        (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("Name like '%" + CadName + "%'");

                         string CadLocation = Convert.ToString(txtSearch.Text);
                        (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("Location like '%" + CadLocation + "%'");


                        string CadSkill = Convert.ToString(txtSearch.Text);
                        (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("Skills like '%" + CadSkill + "%'");


                        string CadInterviewDateTime = Convert.ToString(txtSearch.Text);
                        (dgvCandList.DataSource as DataTable).DefaultView.RowFilter = String.Format("InterviewDateTime like '%" + CadInterviewDateTime + "%'");

            */


            
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

            txtSearch.Text = " ";


            dgvCandList.Focus();

        }




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

            //JsonOperations.ReadSchedulers().Schedulers.OrderBy(Schedulers => Schedulers.Name).ToList();
        }

        private void cmbInterviewerNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            DefaultInterViewerColorCode = ((InterViewScheduler.Interviewers)cmbInterviewerNames.SelectedItem).InterViewerColorCode;
        }

        private void addRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        private void addInterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InterviewerAdd interviewerAdd = new InterviewerAdd();
            interviewerAdd.ShowDialog();
        }

        private void dgvCandList_CellClick(object sender, DataGridViewCellEventArgs e)
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
                //dtpInterviewDate.Text = row.Cells[12].Value.ToString();
                cmbInterviewerNames.Text = row.Cells[13].Value.ToString();
                txtAttendes.Text = row.Cells[14].Value.ToString();
                txtResumeLink.Text = row.Cells[15].Value.ToString();
                txtFeedbackLink.Text = row.Cells[16].Value.ToString();
                cmbDuration.Text = row.Cells[17].Value.ToString();
                txtGoogleMeetUrl.Text = row.Cells[18].Value.ToString();
            }
            catch
            {
                MessageBox.Show("Invalid cell");
            }



        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {

            dgvCandList.Rows.RemoveAt(this.dgvCandList.SelectedRows[0].Index);
            DialogResult result = MessageBox.Show("Do you want to delete this candidate details?", "Delete", MessageBoxButtons.YesNo);
            MessageBox.Show("Candidate Details Deleted Suceessfully");

            /* DialogResult result = MessageBox.Show("Do you want to delete this candidate details?", "Delete", MessageBoxButtons.YesNo);
             if (result == DialogResult.Yes)
             {
                 if (this.dgvCandList.SelectedRows.Count > 0)
                 {
                     dgvCandList.Rows.RemoveAt(this.dgvCandList.SelectedRows[0].Index);
                 }
                 MessageBox.Show("Candidate Details Deleted Suceessfully");
             }*/

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
