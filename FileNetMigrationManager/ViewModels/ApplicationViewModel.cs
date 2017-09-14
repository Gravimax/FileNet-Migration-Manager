using FileNet.Api.Admin;
using FileNet.Api.Collection;
using FileNet.Api.Constants;
using FileNet.Api.Core;
using FileNet.Api.Exception;
using FileNetMigrationManager.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace FileNetMigrationManager
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary> Source FileNet object. </summary>
        FileNetConnect FNSrcObj;

        /// <summary> Destination FileNet object. </summary>
        FileNetConnect FNDestObj;

        DBBusiness recordAudit = new DBBusiness();

        private bool isFullProduct = false;
        private const int maxDemoCount = 10;

        private System.Windows.Visibility _addKeyVisible = System.Windows.Visibility.Visible;

        public System.Windows.Visibility AddKeyVisible
        {
            get { return _addKeyVisible; }
            set
            {
                _addKeyVisible = value;
                OnPropertyChanged("AddKeyVisible");
            }
        }


        #region Object Stores

        private IObjectStoreSet _fnSrcObjectStores;
        /// <summary> The object stores list. </summary>
        public IObjectStoreSet FNSrcObjectStores
        {
            get { return _fnSrcObjectStores; }
            set
            {
                _fnSrcObjectStores = value;
                OnPropertyChanged("FNSrcObjectStores");
            }
        }

        private IObjectStore _fNSrcObjectStore;
        /// <summary>  Selected Source object store. </summary>
        public IObjectStore FNSrcObjectStore
        {
            get { return _fNSrcObjectStore; }
            set
            {
                _fNSrcObjectStore = value;
                OnPropertyChanged("FNSrcObjectStore");
            }
        }

        private IObjectStoreSet _fnDestObjectStores;
        /// <summary> The object stores list. </summary>
        public IObjectStoreSet FNDestObjectStores
        {
            get { return _fnDestObjectStores; }
            set
            {
                _fnDestObjectStores = value;
                OnPropertyChanged("FNDestObjectStores");
            }
        }

        private IObjectStore _fNDestObjectStore;
        /// <summary> Selected Destination object store. </summary>
        public IObjectStore FNDestObjectStore
        {
            get { return _fNDestObjectStore; }
            set
            {
                _fNDestObjectStore = value;
                OnPropertyChanged("FNDestObjectStore");
            }
        }

        #endregion


        #region Classes

        private List<FNClassItem> _fNSrcClasses;
        /// <summary> The Source class objects. </summary>
        public List<FNClassItem> FNSrcClasses
        {
            get { return _fNSrcClasses; }
            set
            {
                _fNSrcClasses = value;
                OnPropertyChanged("FNSrcClasses");
            }
        }

        private FNClassItem _fNSrcCLSObj;
        /// <summary> Selected Source class object. </summary>
        public FNClassItem FNSrcCLSObj
        {
            get { return _fNSrcCLSObj; }
            set
            {
                _fNSrcCLSObj = value;
                OnPropertyChanged("FNSrcCLSObj");
            }
        }

        private List<FNClassItem> _fNDestClasses;
        /// <summary> The Destination class objects. </summary>
        public List<FNClassItem> FNDestClasses
        {
            get { return _fNDestClasses; }
            set
            {
                _fNDestClasses = value;
                OnPropertyChanged("FNDestClasses");
            }
        }

        private FNClassItem _fNDestCLSObj;
        /// <summary> Selected Destination class object. </summary>
        public FNClassItem FNDestCLSObj
        {
            get { return _fNDestCLSObj; }
            set
            {
                _fNDestCLSObj = value;
                OnPropertyChanged("FNDestCLSObj");
            }
        }

        #endregion


        #region Properties

        private List<FNProperty> _fNSrcProperties;
        /// <summary>List of the FileNet Source properties. </summary>
        public List<FNProperty> FNSrcProperties
        {
            get { return _fNSrcProperties; }
            set
            {
                _fNSrcProperties = value;
                OnPropertyChanged("FNSrcProperties");
            }
        }

        private FNProperty _srcProperty;

        public FNProperty SrcProperty
        {
            get { return _srcProperty; }
            set
            {
                _srcProperty = value;
                OnPropertyChanged("SrcProperty");
            }
        }

        private List<FNProperty> _fNDestProperties;
        /// <summary>List of the FileNet destination properties. </summary>
        public List<FNProperty> FNDestProperties
        {
            get { return _fNDestProperties; }
            set
            {
                _fNDestProperties = value;
                OnPropertyChanged("FNDestProperties");
            }
        }

        private FNProperty _destProperty;

        public FNProperty DestProperty
        {
            get { return _destProperty; }
            set
            {
                _destProperty = value;
                OnPropertyChanged("DestProperty");
            }
        }

        private PropertiesMap _seletedProperty;

        public PropertiesMap SelectedProperty
        {
            get { return _seletedProperty; }
            set
            {
                _seletedProperty = value;
                OnPropertyChanged("SelectedProperty");
            }
        }

        #endregion


        #region Other

        /// <summary> Counts of the documents for the summary </summary>
        int TotalCount = 0;
        int SuccessCount = 0;
        int CurrentVersionCount = 0;
        int VersionCount = 0;
        int ChildCount = 0;
        int FailCount = 0;
        int AlreadyProcessedCount = 0;
        bool checkExceptionLog = false;
        
        private int tzOffset = 5; // Timezone offset for UTC. Defaults to users timezone offset.

        private List<string> _mtomUrls;

        public List<string> MtomUrls
        {
            get
            {
                return _mtomUrls;
            }
            set
            {
                _mtomUrls = value;
                OnPropertyChanged("MtomUrls");
            }
        }

        private ProjectTemplate _currentTemplate;

        public ProjectTemplate CurrentTemplate
        {
            get { return _currentTemplate; }
            set
            {
                _currentTemplate = value;
                OnPropertyChanged("CurrentTemplate");
            }
        }

        private string _selectedTemplate;

        public string SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate != value)
                {
                    _selectedTemplate = value;
                    OnPropertyChanged("SelectedTemplate");
                }
            }
        }

        private ObservableCollection<string> _templateList;

        public ObservableCollection<string> TemplateList
        {
            get { return _templateList; }
            set
            {
                _templateList = value;
                OnPropertyChanged("TemplateList");
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private DatabaseConfiguration dbConfig;

        #endregion


        LoadingForm loadingForm = null;

        #endregion


        #region Delegate Commands

        public DelegateCommand GetSrcObjectStoresCommand { get; private set; }
        public DelegateCommand GetDestObjectStoresCommand { get; private set; }
        public DelegateCommand AddPropertyToListCommand { get; private set; }
        public DelegateCommand AddSameAsPropertiesCommand { get; private set; }
        public DelegateCommand RemovePropertyCommand { get; private set; }
        public DelegateCommand ResetPropertyListCommand { get; private set; }
        public DelegateCommand SaveTemplateCommand { get; private set; }
        public DelegateCommand LoadSelTempCommand { get; private set; }
        public DelegateCommand NewTemplateCommand { get; private set; }
        public DelegateCommand ReloadTemplateCommand { get; private set; }
        public DelegateCommand EditDBConnectionCommand { get; private set; }
        public DelegateCommand StartMigrationCommand { get; private set; }
        public DelegateCommand ExitApplicationCommand { get; private set; }
        public DelegateCommand ShowAboutCommand { get; private set; }
        public DelegateCommand ShowDocsCommand { get; private set; }

        public DelegateCommand SrcObjStoreChangedCommand { get; private set; }
        public DelegateCommand DestObjStoreChangedCommand { get; private set; }
        public DelegateCommand SrcClassChangedCommand { get; private set; }
        public DelegateCommand DestClassChangedCommand { get; private set; }
        public DelegateCommand SrcKeyUpCommand { get; private set; }
        public DelegateCommand DestKeyUpCommand { get; private set; }
        public DelegateCommand SrcPasswordChangedCommand { get; private set; }
        public DelegateCommand DestPasswordChangedCommand { get; private set; }

        public DelegateCommand SrcUrlLostFocusCommand { get; private set; }
        public DelegateCommand DestUrlLostFocusCommand { get; private set; }
        public DelegateCommand CopyCredsCommand { get; private set; }

        public DelegateCommand ViewAuditListCommand { get; private set; }

        public DelegateCommand AddProductKeyCommand { get; private set; }

        #endregion


        #region Ctors


        public ApplicationViewModel()
        {
            try
            {
                GetSrcObjectStoresCommand = new DelegateCommand(GetSrcObjectStores);
                GetDestObjectStoresCommand = new DelegateCommand(GetDestObjectStores);
                AddPropertyToListCommand = new DelegateCommand(AddPropertyToList);
                AddSameAsPropertiesCommand = new DelegateCommand(AddSameAsProperties, CanAddSameAsProperties);
                RemovePropertyCommand = new DelegateCommand(RemoveSelectedProperty);
                ResetPropertyListCommand = new DelegateCommand(ResetPropertiesList);

                SaveTemplateCommand = new DelegateCommand(SaveTemplate);
                LoadSelTempCommand = new DelegateCommand(LoadTemplate);
                NewTemplateCommand = new DelegateCommand(NewTemplate);
                ReloadTemplateCommand = new DelegateCommand(ReloadTemplate);

                EditDBConnectionCommand = new DelegateCommand(EditDBConnection);
                StartMigrationCommand = new DelegateCommand(StartMigration);

                ExitApplicationCommand = new DelegateCommand(ExitApp);
                ShowAboutCommand = new DelegateCommand(ShowAbout);
                ShowDocsCommand = new DelegateCommand(ShowDocs);

                SrcObjStoreChangedCommand = new DelegateCommand(SrcObjStoreChanged);
                DestObjStoreChangedCommand = new DelegateCommand(DestObjStoreChanged);
                SrcClassChangedCommand = new DelegateCommand(SrcClassChanged);
                DestClassChangedCommand = new DelegateCommand(DestClassChanged);

                SrcKeyUpCommand = new DelegateCommand(SrcKeyUp);
                DestKeyUpCommand = new DelegateCommand(DestKeyUp);
                SrcPasswordChangedCommand = new DelegateCommand(SrcPasswordChanged);
                DestPasswordChangedCommand = new DelegateCommand(DestPasswordChanged);

                SrcUrlLostFocusCommand = new DelegateCommand(SrcUrlLostFocus);
                DestUrlLostFocusCommand = new DelegateCommand(DestUrlLostFocus);
                CopyCredsCommand = new DelegateCommand(CopyCreds);
                ViewAuditListCommand = new DelegateCommand(ViewAuditList);
                AddProductKeyCommand = new DelegateCommand(AddProductKey);

                dbConfig = Utilities.LoadDatabaseConfig();

                MtomUrls = Utilities.LoadUrlList();
                TemplateList = Utilities.GetTemplateList().ToObservableCollection();
                CurrentTemplate = new ProjectTemplate();
                CurrentTemplate.PropertiesMapping = new System.Collections.ObjectModel.ObservableCollection<PropertiesMap>();
                recordAudit.LoadAuditTable();
                RunValidation();
                //TestStartup();
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
            }
        }

        private void RunValidation()
        {
            try
            {
                Registration.ProductKeyManager keyMan = new Registration.ProductKeyManager();
                string keyValue;
                isFullProduct = keyMan.CheckKey(out keyValue);
                if (isFullProduct) { AddKeyVisible = System.Windows.Visibility.Collapsed; }
            }
            catch (ProductKeyInvalidException pkie)
            {
                Utilities.ShowErrorMessageBox(pkie.Message);
            }
            catch (Exception ex)
            {
                Utilities.WriteToExceptionLog(ex, "There was an error accessing the activation Key file");
                Utilities.ShowExceptionMessageBox(ex);
            }
        }

        private System.Windows.Window inputOwner = null;
        private void AddProductKey(object args)
        {
            try
            {
                Registration.ProductKeyManager keyMan = new Registration.ProductKeyManager();

                InputBox inputBox = new InputBox(string.Empty, "Enter Activation Key");

                inputOwner = args as System.Windows.Window;
                if (inputOwner != null)
                {
                    inputBox.Owner = inputOwner;
                }

                if (inputBox.ShowDialog() == true)
                {
                    isFullProduct = keyMan.AddNewKey(inputBox.Value.Trim());
                    AddKeyVisible = System.Windows.Visibility.Collapsed;
                }
            }
            catch (ProductKeyInvalidException pkie)
            {
                Utilities.ShowErrorMessageBox(pkie.Message);
                AddProductKey(inputOwner);
            }
            catch (Exception ex)
            {
                Utilities.WriteToExceptionLog(ex, "There was an error processing the activation key");
                Utilities.ShowExceptionMessageBox(ex);
            }
            finally
            {
                inputOwner = null;
            }
        }

        private void TestStartup()
        {
            for (int i = 0; i <= 10; i++)
            {
                recordAudit.AuditDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, "", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "");
            }

            recordAudit.SaveAuditTable();
        }


        #endregion


        #region Load/Save/Etc

        private void ShowAbout(object args)
        {
            About about = new About();

            System.Windows.Window owner = args as System.Windows.Window;
            if (owner != null)
            {
                about.Owner = owner;
            }

            about.ShowDialog();
        }

        private void ViewAuditList(object args)
        {
            Views.AuditListViewer viewer = new Views.AuditListViewer(recordAudit.AuditList);

            System.Windows.Window owner = args as System.Windows.Window;
            if (owner != null)
            {
                viewer.Owner = owner;
            }

            viewer.ShowDialog();
        }

        private void ShowDocs(object args)
        {
            if (Utilities.FileExists(System.IO.Path.Combine(Utilities.GetCurrentDirectory(), "User Guide", "index.html")))
            {
                System.Diagnostics.Process.Start(System.IO.Path.Combine(Utilities.GetCurrentDirectory(), "User Guide", "index.html"));
            }
            else
            {
                Utilities.ShowWarningMessageBox("User Guide not found!");
            }
        }

        private void ExitApp(object args)
        {
            OnExitApplication();
        }

        private void NewTemplate(object args)
        {
            if (Utilities.ShowDialogBox("Save current template?") == true)
            {
                SaveTemplate(args);
            }

            CurrentTemplate = new ProjectTemplate
            {
                FromDate = DateTime.Now,
                IsAllVersions = 0,
                IsMove = 0,
                ToDate = DateTime.Now,
                PropertiesMapping = new System.Collections.ObjectModel.ObservableCollection<PropertiesMap>()
            };

            ClearForm();
        }

        public void SaveTemplate(object args)
        {
            try
            {
                InputBox inputBox = new InputBox(CurrentTemplate.TemplateName);

                System.Windows.Window owner = args as System.Windows.Window;
                if (owner != null)
                {
                    inputBox.Owner = owner;
                }

                if (inputBox.ShowDialog() == true)
                {
                    _currentTemplate.TemplateName = inputBox.Value;

                    if (System.IO.File.Exists(Utilities.GetTemplateFilePath(inputBox.Value)))
                    {
                        if (Utilities.ShowDialogBox("Overwrite template?") == false)
                        {
                            return;
                        }
                    }

                    Utilities.SerializeToFile<ProjectTemplate>(Utilities.GetTemplateFilePath(inputBox.Value), _currentTemplate);
                    Message = "Template saved";

                    if (TemplateList.FirstOrDefault(x => x == _currentTemplate.TemplateName) == null) // Add to list if new template name
                    {
                        TemplateList.Add(_currentTemplate.TemplateName);
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
            }
        }

        //private void LoadSelectedTemplate(object args)
        //{
        //    try
        //    {
        //        if (Utilities.ShowDialogBox("Save current template?") == true)
        //        {
        //            SaveTemplate(args);
        //        }

        //        LoadTemplate loadTemplate = new LoadTemplate();

        //        System.Windows.Window owner = args as System.Windows.Window;
        //        if (owner != null)
        //        {
        //            loadTemplate.Owner = owner;
        //        }

        //        if (loadTemplate.ShowDialog() == true)
        //        {
        //            var temp = loadTemplate.SelectedTemplate;

        //            CurrentTemplate = temp;

        //            if (!string.IsNullOrEmpty(temp.SourceUrl) && (MtomUrls.FirstOrDefault(x => x == temp.SourceUrl) == null))
        //            {
        //                MtomUrls.Add(temp.SourceUrl);
        //            }

        //            if (!string.IsNullOrEmpty(temp.DestinationUrl) && (MtomUrls.FirstOrDefault(x => x == temp.DestinationUrl) == null))
        //            {
        //                MtomUrls.Add(temp.DestinationUrl);
        //            }

        //            SetTemplate(_currentTemplate, args);
        //            AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
        //        }

        //        TemplateList = Utilities.GetTemplateList().ToObservableCollection();
        //    }
        //    catch (Exception ex)
        //    {
        //        Utilities.ShowExceptionMessageBox(ex);
        //    }
        //}

        private void LoadTemplate(object args)
        {
            try
            {
                if (Utilities.FileExists(System.IO.Path.Combine(Utilities.GetTemplateFilePath(_selectedTemplate))))
                {
                    var temp = Utilities.DeserializeFromFile<ProjectTemplate>(System.IO.Path.Combine(Utilities.GetTemplateFilePath(_selectedTemplate)));

                    CurrentTemplate = temp;

                    if (!string.IsNullOrEmpty(temp.SourceUrl) && (MtomUrls.FirstOrDefault(x => x == temp.SourceUrl) == null))
                    {
                        MtomUrls.Add(temp.SourceUrl);
                    }

                    if (!string.IsNullOrEmpty(temp.DestinationUrl) && (MtomUrls.FirstOrDefault(x => x == temp.DestinationUrl) == null))
                    {
                        MtomUrls.Add(temp.DestinationUrl);
                    }

                    SetTemplate(_currentTemplate, args);
                    AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
                }
                else // Remove the template if it does not exist.
                {
                    TemplateList.Remove(_selectedTemplate);
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
            }
        }

        private void ReloadTemplate(object args)
        {
            if (!string.IsNullOrEmpty(_selectedTemplate))
            {
                CurrentTemplate = null;
                LoadTemplate(args);
            }
        }

        private void SetTemplate(ProjectTemplate template, object args)
        {
            OnSetSrcPassword(template.SrcPassword);
            OnSetDestPassword(template.DestPassword);

            FNSrcObjectStores = null;
            FNDestObjectStores = null;
            FNSrcObjectStore = null;
            FNDestObjectStore = null;
            FNSrcClasses = null;
            FNDestClasses = null;
            FNSrcCLSObj = null;
            FNDestCLSObj = null;
            FNSrcObj = null;
            FNDestObj = null;
            FNSrcObj = null;
            FNDestObj = null;

            loadingForm = new LoadingForm("", "Loading Template");

            System.Windows.Window owner = args as System.Windows.Window;
            if (owner != null)
            {
                loadingForm.Owner = owner;
            }
            else
            {
                loadingForm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }

            loadingForm.Loaded += (s, e) =>
            {
                BackgroundWorker worker = new BackgroundWorker();
                
                loadingForm.CancelOperation += (ss, ee) =>
                {
                    worker.CancelAsync();
                    Message = "Process canceled";
                };

                IProgress<string> progress = new Progress<string>(data => loadingForm.SetMessage(data));

                worker.DoWork += (ss, workerArgs) =>
                {
                    LoadSource(template, progress);
                    LoadDestination(template, progress);
                };

                worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                worker.RunWorkerAsync();
            };

            loadingForm.ShowDialog();
        }

        private void LoadSource(ProjectTemplate template, IProgress<string> progress)
        {
            if (!string.IsNullOrEmpty(template.SrcUserName) && !string.IsNullOrEmpty(template.SrcPassword) && !string.IsNullOrEmpty(template.SourceUrl))
            {
                FNSrcObj = new FileNetConnect(template.SrcUserName, template.SrcPassword, template.SourceUrl);
            }

            // Get the source items if selected
            if (!string.IsNullOrEmpty(template.SelectedSrcObjectStore) && FNSrcObj != null)
            {
                progress.Report("Getting the source object stores");
                FNSrcObjectStores = FNSrcObj.GetObjectStores();

                foreach (IObjectStore item in FNSrcObjectStores)
                {
                    if (item.SymbolicName == template.SelectedSrcObjectStore)
                    {
                        FNSrcObjectStore = item;

                        progress.Report("Getting the source classes");
                        var ParentDocCLSObj = Factory.ClassDefinition.FetchInstance(item, "Document", null);

                        List<FNClassItem> temp = new List<FNClassItem>();

                        if (ParentDocCLSObj.ImmediateSubclassDefinitions != null)
                        {
                            FNClassItem classItem = ClassDefToItem(ParentDocCLSObj, 0);
                            temp.Add(classItem);
                            GetSubClasses(classItem, temp);
                        }

                        FNSrcClasses = temp;

                        foreach (FNClassItem srcClass in FNSrcClasses)
                        {
                            if (srcClass.SymbolicName == template.SelectedSrcClass)
                            {
                                progress.Report("Getting the source properties");
                                var props = srcClass.ClassDef.PropertyDefinitions;

                                FNSrcProperties = PropertiesToList(props);
                                FNSrcCLSObj = srcClass;
                            }
                        }
                    }
                }
            }
        }

        private void LoadDestination(ProjectTemplate template, IProgress<string> progress)
        {
            if (!string.IsNullOrEmpty(template.DestUserName) && !string.IsNullOrEmpty(template.DestPassword) && !string.IsNullOrEmpty(template.DestinationUrl))
            {
                FNDestObj = new FileNetConnect(template.DestUserName, template.DestPassword, template.DestinationUrl);
            }

            // Get the destination items if selected
            if (!string.IsNullOrEmpty(template.SelectedDestObjectStore) && FNDestObj != null)
            {
                progress.Report("Getting the destination object stores");
                FNDestObjectStores = FNDestObj.GetObjectStores();

                foreach (IObjectStore item in FNDestObjectStores)
                {
                    if (item.SymbolicName == template.SelectedDestObjectStore)
                    {
                        FNDestObjectStore = item;

                        progress.Report("Getting the destination classes");
                        var ParentDocCLSObj = Factory.ClassDefinition.FetchInstance(item, "Document", null);

                        List<FNClassItem> temp = new List<FNClassItem>();

                        if (ParentDocCLSObj.ImmediateSubclassDefinitions != null)
                        {
                            FNClassItem classItem = ClassDefToItem(ParentDocCLSObj, 0);
                            temp.Add(classItem);
                            GetSubClasses(classItem, temp);
                        }

                        FNDestClasses = temp;

                        foreach (FNClassItem destClass in FNDestClasses)
                        {
                            if (destClass.SymbolicName == template.SelectedDestClass)
                            {
                                progress.Report("Getting the destination properties");
                                var props = destClass.ClassDef.PropertyDefinitions;

                                FNDestProperties = PropertiesToList(props);
                                FNDestCLSObj = destClass;
                            }
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            FNSrcObjectStores = null;
            FNDestObjectStores = null;
            FNSrcObjectStore = null;
            FNDestObjectStore = null;
            FNSrcClasses = null;
            FNDestClasses = null;
            FNSrcCLSObj = null;
            FNDestCLSObj = null;
            FNSrcObj = null;
            FNDestObj = null;
            FNSrcObj = null;
            FNDestObj = null;
        }

        private void EditDBConnection(object args)
        {
            try
            {
                DatabaseConfig config = new DatabaseConfig(recordAudit);

                System.Windows.Window owner = args as System.Windows.Window;
                if (owner != null)
                {
                    config.Owner = owner;
                }

                config.ShowDialog();
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
            }
        }

        #endregion


        #region Support

        private void CopyCreds(object args)
        {
            CurrentTemplate.DestinationUrl = CurrentTemplate.SourceUrl;
            CurrentTemplate.DestPassword = CurrentTemplate.SrcPassword;
            CurrentTemplate.DestUserName = CurrentTemplate.SrcUserName;
            OnSetDestPassword(CurrentTemplate.SrcPassword);
        }

        private void SrcUrlLostFocus(object args)
        {
            string url = args as string;
            if (!string.IsNullOrEmpty(url))
            {
                if (MtomUrls.FirstOrDefault(x => x == url) == null)
                {
                    MtomUrls.Add(url);
                    CurrentTemplate.SourceUrl = url;
                }
            }
        }

        private void DestUrlLostFocus(object args)
        {
            string url = args as string;
            if (!string.IsNullOrEmpty(url))
            {
                if (MtomUrls.FirstOrDefault(x => x == url) == null)
                {
                    MtomUrls.Add(url);
                    CurrentTemplate.DestinationUrl = url;
                }
            }
        }

        private bool HasCredentials(string url, string username, string password)
        {
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(url);
        }

        #endregion


        #region Object Stores

        /// <summary>
        /// Get the Source object stores
        /// </summary>
        private void GetSrcObjectStores(object args)
        {
            try
            {
                if (FNSrcObj == null)
                {
                    FNSrcObj = new FileNetConnect(_currentTemplate.SrcUserName, _currentTemplate.SrcPassword, _currentTemplate.SourceUrl);
                }
                else
                {
                    FNSrcObj.EstablishConnection(false);
                }

                loadingForm = new LoadingForm("", "Getting the source object stores");
                System.Windows.Window owner = args as System.Windows.Window;
                if (owner != null)
                {
                    loadingForm.Owner = owner;
                }

                loadingForm.Loaded += (s, e) =>
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    loadingForm.CancelOperation += (ss, ee) =>
                    {
                        worker.CancelAsync();
                        Message = "Process canceled";
                    };

                    worker.DoWork += (ss, workerArgs) => {
                        FNSrcObjectStores = FNSrcObj.GetObjectStores();
                        FNSrcClasses = null;
                        FNSrcCLSObj = null;
                    };

                    worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                    worker.RunWorkerAsync();
                };

                loadingForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
            }
        }

        /// <summary>
        /// Get the Dest object stores
        /// </summary>
        private void GetDestObjectStores(object args)
        {
            try
            {
                if (FNDestObj == null)
                {
                    FNDestObj = new FileNetConnect(_currentTemplate.DestUserName, _currentTemplate.DestPassword, _currentTemplate.DestinationUrl);
                }
                else
                {
                    FNDestObj.EstablishConnection(false);
                }

                loadingForm = new LoadingForm("", "Getting the destination object stores");
                System.Windows.Window owner = args as System.Windows.Window;
                if (owner != null)
                {
                    loadingForm.Owner = owner;
                }

                loadingForm.Loaded += (s, e) =>
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    loadingForm.CancelOperation += (ss, ee) =>
                    {
                        worker.CancelAsync();
                        Message = "Process canceled";
                    };

                    worker.DoWork += (ss, workerArgs) =>
                    {
                        FNDestObjectStores = FNDestObj.GetObjectStores();
                        FNDestClasses = null;
                        FNDestCLSObj = null;
                    };

                    worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                    worker.RunWorkerAsync();
                };

                loadingForm.Closed += (s, e) =>
                {
                    AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
                };

                loadingForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
            }
        }

        #endregion


        #region Classes

        private void GetSourceClasses(IObjectStore objStore, object args)
        {
            try
            {
                if (objStore != null)
                {
                    loadingForm = new LoadingForm("", "Getting the source classes");

                    System.Windows.Window owner = args as System.Windows.Window;
                    if (owner != null)
                    {
                        loadingForm.Owner = owner;
                    }

                    loadingForm.Loaded += (s, e) =>
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        loadingForm.CancelOperation += (ss, ee) =>
                        {
                            worker.CancelAsync();
                            Message = "Process canceled";
                        };

                        worker.DoWork += (ss, workerArgs) => {
                            var ParentDocCLSObj = Factory.ClassDefinition.FetchInstance(objStore, "Document", null);

                            List<FNClassItem> temp = new List<FNClassItem>();

                            if (ParentDocCLSObj.ImmediateSubclassDefinitions != null)
                            {
                                FNClassItem classItem = ClassDefToItem(ParentDocCLSObj, 0);
                                temp.Add(classItem);
                                GetSubClasses(classItem, temp);
                            }

                            FNSrcClasses = temp;
                        };

                        worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                        worker.RunWorkerAsync();
                    };

                    loadingForm.ShowDialog();
                    AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
            }
        }

        private void GetDestinationClasses(IObjectStore objStore, object args)
        {
            try
            {
                if (objStore != null)
                {
                    loadingForm = new LoadingForm("", "Getting the destination classes");

                    System.Windows.Window owner = args as System.Windows.Window;
                    if (owner != null)
                    {
                        loadingForm.Owner = owner;
                    }

                    loadingForm.Loaded += (s, e) =>
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        loadingForm.CancelOperation += (ss, ee) =>
                        {
                            worker.CancelAsync();
                            Message = "Process canceled";
                        };

                        worker.DoWork += (ss, workerArgs) => {
                            var ParentDocCLSObj = Factory.ClassDefinition.FetchInstance(objStore, "Document", null);

                            List<FNClassItem> temp = new List<FNClassItem>();

                            if (ParentDocCLSObj.ImmediateSubclassDefinitions != null)
                            {
                                FNClassItem classItem = ClassDefToItem(ParentDocCLSObj, 0);
                                temp.Add(classItem);
                                GetSubClasses(classItem, temp);
                            }

                            FNDestClasses = temp;
                        };

                        worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                        worker.RunWorkerAsync();
                    };

                    loadingForm.ShowDialog();
                    AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
            }
        }

        // Get classes and sub classes recursivly.
        private void GetSubClasses(FNClassItem pClass, List<FNClassItem> temp)
        {
            if (pClass.ClassDef.ImmediateSubclassDefinitions != null)
            {
                foreach (IClassDefinition item in pClass.ClassDef.ImmediateSubclassDefinitions)
                {
                    FNClassItem classItem = ClassDefToItem(item, pClass.Level + 1);

                    temp.Add(classItem);

                    if (item.ImmediateSubclassDefinitions != null)
                    {
                        GetSubClasses(classItem, temp);
                    }
                }
            }
        }

        private FNClassItem ClassDefToItem(IClassDefinition pClass, int level)
        {
            return new FNClassItem
            {
                ClassDef = pClass,
                DisplayName = new string(' ', level * 4) + pClass.DisplayName,
                SymbolicName = pClass.SymbolicName,
                Level = level
            };
        }

        #endregion


        #region Folders

        private List<IFolder> GetFolders(IObjectStore objStore)
        {
            List<IFolder> temp = new List<IFolder>();

            if (objStore.RootFolder != null)
            {
                var rootFolder = objStore.RootFolder;
                temp.Add(rootFolder);

                GetSubFolders(rootFolder, temp);
            }

            return temp;
        }

        // Gets sub folders recursively
        private void GetSubFolders(IFolder folder, List<IFolder> temp)
        {
            if (folder.SubFolders != null)
            {
                foreach (IFolder item in folder.SubFolders)
                {
                    temp.Add(item);

                    if (item.SubFolders != null)
                    {
                        GetSubFolders(item, temp);
                    }
                }
            }
        }

        #endregion


        #region Properties

        private void GetSrcProperties(IClassDefinition srcClass, object args)
        {
            try
            {
                if (srcClass != null)
                {
                    loadingForm = new LoadingForm("", "Getting the source classes");

                    System.Windows.Window owner = args as System.Windows.Window;
                    if (owner != null)
                    {
                        loadingForm.Owner = owner;
                    }

                    loadingForm.Loaded += (s, e) =>
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        loadingForm.CancelOperation += (ss, ee) =>
                        {
                            worker.CancelAsync();
                            Message = "Process canceled";
                        };

                        worker.DoWork += (ss, workerArgs) => {
                            var props = srcClass.PropertyDefinitions;

                            FNSrcProperties = PropertiesToList(props);
                        };

                        worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                        worker.RunWorkerAsync();
                    };

                    loadingForm.ShowDialog();
                    AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
            }
        }

        private void GetDestProperties(IClassDefinition destClass, object args)
        {
            try
            {
                if (destClass != null)
                {
                    loadingForm = new LoadingForm("", "Getting the source classes");

                    System.Windows.Window owner = args as System.Windows.Window;
                    if (owner != null)
                    {
                        loadingForm.Owner = owner;
                    }

                    loadingForm.Loaded += (s, e) =>
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        loadingForm.CancelOperation += (ss, ee) =>
                        {
                            worker.CancelAsync();
                            Message = "Process canceled";
                        };

                        worker.DoWork += (ss, workerArgs) => {
                            var props = destClass.PropertyDefinitions;

                            FNDestProperties = PropertiesToList(props);
                        };

                        worker.RunWorkerCompleted += (ss, workerArgs) => loadingForm.Close();
                        worker.RunWorkerAsync();
                    };

                    loadingForm.ShowDialog();
                    AddSameAsPropertiesCommand.RaiseCanExecuteChanged();
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
            }
        }

        private List<FNProperty> PropertiesToList(IPropertyDefinitionList props)
        {
            List<FNProperty> properties = new List<FNProperty>();

            foreach (IPropertyDefinition item in props)
            {
                if (item.IsSystemOwned == false && item.IsHidden == false && item.CopyToReservation == true)
                {
                    properties.Add(new FNProperty
                    {
                        Cardinality = item.Cardinality,
                        DataType = item.DataType,
                        DisplayName = string.Format("{0} ({1} - {2} - {3})", item.DisplayName, item.DataType.ToString(), item.Cardinality.ToString(), (item.IsValueRequired == true ? "Required" : "Optional")),
                        IsValueRequired = item.IsValueRequired,
                        SymbolicName = item.SymbolicName
                    });
                }
            }

            properties = properties.OrderBy(x => x.DisplayName).ToList();
            return properties;
        }

        private bool CanAddPropertyToList(object args)
        {
            return SrcProperty != null && DestProperty != null;
        }

        private void AddPropertyToList(object args)
        {
            try
            {
                if (DestProperty == null || SrcProperty == null)
                {
                    Utilities.ShowWarningMessageBox(string.Format("Please select the source and destination properties."));
                    return;
                }
                else if (SrcProperty.DataType != DestProperty.DataType) // Check the data types //  || SrcProperty.Cardinality != DestProperty.Cardinality
                {
                    Utilities.ShowWarningMessageBox(string.Format("The data types are mismatched for the selected properties."));
                    return;
                }
                else if (SrcProperty.Cardinality == FileNet.Api.Constants.Cardinality.LIST && (DestProperty.Cardinality == FileNet.Api.Constants.Cardinality.SINGLE || DestProperty.Cardinality == FileNet.Api.Constants.Cardinality.ENUM))
                {
                    Utilities.ShowWarningMessageBox(string.Format("Cardinality can not go from multi to single."));
                    return;
                }
                else if (CurrentTemplate.PropertiesMapping.FirstOrDefault(x => x.DestinationProperty.DisplayName == DestProperty.DisplayName && x.SourceProperty.DisplayName == SrcProperty.DisplayName) != null) // Check if this map already exists
                {
                    Utilities.ShowWarningMessageBox(string.Format("The same mapping already exists."));
                    return;
                }
                else if (CurrentTemplate.PropertiesMapping.FirstOrDefault(x => x.DestinationProperty.SymbolicName == DestProperty.SymbolicName) != null) // Check if the destination property is already exists
                {
                    Utilities.ShowWarningMessageBox(string.Format("The destination property is already mapped to another property."));
                    return;
                }

                CurrentTemplate.PropertiesMapping.Add(new PropertiesMap { DestinationProperty = DestProperty, SourceProperty = SrcProperty });

                SrcProperty = null;
                DestProperty = null;
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
            }
        }

        private bool CanAddSameAsProperties(object args)
        {
            return FNSrcObjectStore != null && FNDestObjectStore != null && FNSrcCLSObj != null && FNDestCLSObj != null;
        }

        // Adds all compatible matching properties
        private void AddSameAsProperties(object args)
        {
            foreach (var srcProp in FNSrcProperties)
            {
                var destProp = FNDestProperties.FirstOrDefault(x => x.SymbolicName == srcProp.SymbolicName);
                if (destProp != null)
                {
                    if (srcProp.DataType != destProp.DataType || srcProp.Cardinality != destProp.Cardinality) // Check the data types
                    {
                        continue;
                    }
                    else if (CurrentTemplate.PropertiesMapping.FirstOrDefault(x => x.DestinationProperty.DisplayName == destProp.DisplayName && x.SourceProperty.DisplayName == srcProp.DisplayName) != null) // Check if this map already exists
                    {
                        continue;
                    }
                    else if (CurrentTemplate.PropertiesMapping.FirstOrDefault(x => x.DestinationProperty.SymbolicName == destProp.SymbolicName) != null) // Check if the destination property already exists
                    {
                        continue;
                    }

                    CurrentTemplate.PropertiesMapping.Add(new PropertiesMap { DestinationProperty = destProp, SourceProperty = srcProp });
                }
            }
        }

        private bool CanRemoveProperty(object args)
        {
            return SelectedProperty != null;
        }

        private void RemoveSelectedProperty(object args)
        {
            if (CanRemoveProperty(null))
            {
                CurrentTemplate.PropertiesMapping.Remove(SelectedProperty);
                SelectedProperty = null;
            }
        }

        private void ResetPropertiesList(object args)
        {
            CurrentTemplate.PropertiesMapping = new System.Collections.ObjectModel.ObservableCollection<PropertiesMap>();
            SelectedProperty = null;
        }

        #endregion


        #region Control Events

        private void SrcObjStoreChanged(object args)
        {
            if (FNSrcObjectStore != null && FNSrcObjectStore.SymbolicName != _currentTemplate.SelectedSrcObjectStore)
            {
                _currentTemplate.SelectedSrcObjectStore = FNSrcObjectStore.SymbolicName;
                GetSourceClasses(FNSrcObjectStore, args);
            }
            else if (FNSrcObjectStore == null)
            {
                _currentTemplate.SelectedSrcObjectStore = string.Empty;
            }
        }

        private void DestObjStoreChanged(object args)
        {
            if (FNDestObjectStore != null && FNDestObjectStore.SymbolicName != _currentTemplate.SelectedDestObjectStore)
            {
                _currentTemplate.SelectedDestObjectStore = FNDestObjectStore.SymbolicName;
                GetDestinationClasses(FNDestObjectStore, args);
            }
            else if (FNDestObjectStore == null)
            {
                _currentTemplate.SelectedDestObjectStore = string.Empty;
            }
        }

        private void SrcClassChanged(object args)
        {
            if (FNSrcCLSObj != null && FNSrcCLSObj.SymbolicName != _currentTemplate.SelectedSrcClass)
            {
                _currentTemplate.SelectedSrcClass = FNSrcCLSObj.SymbolicName;
                GetSrcProperties(FNSrcCLSObj.ClassDef, args);
            }
            else if (FNSrcCLSObj == null)
            {
                _currentTemplate.SelectedSrcClass = string.Empty;
            }
        }

        private void DestClassChanged(object args)
        {
            if (FNDestCLSObj != null && FNDestCLSObj.SymbolicName != _currentTemplate.SelectedDestClass)
            {
                _currentTemplate.SelectedDestClass = FNDestCLSObj.SymbolicName;
                GetDestProperties(FNDestCLSObj.ClassDef, args);
            }
            else if (FNDestCLSObj == null)
            {
                _currentTemplate.SelectedDestClass = string.Empty;
            }
        }

        private void SrcKeyUp(object args)
        {
            KeyEventArgs e = args as KeyEventArgs;
            if (e.Key == Key.Enter)
            {
                System.Windows.Window parent = null;

                if (e.OriginalSource is System.Windows.Controls.TextBox)
                {
                    System.Windows.Controls.TextBox sender = e.OriginalSource as System.Windows.Controls.TextBox;
                    parent = System.Windows.Window.GetWindow(sender);
                }
                else
                {
                    System.Windows.Controls.PasswordBox sender = e.OriginalSource as System.Windows.Controls.PasswordBox;
                    parent = System.Windows.Window.GetWindow(sender);
                }

                // If nothing has changed do nothing
                if (FNSrcObjectStore != null && FNSrcObjectStore.SymbolicName == _currentTemplate.SelectedSrcObjectStore)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(_currentTemplate.SourceUrl) && !string.IsNullOrEmpty(_currentTemplate.SrcUserName) && !string.IsNullOrEmpty(_currentTemplate.SrcPassword))
                {
                    GetSrcObjectStores(parent);
                }
            }
        }

        private void DestKeyUp(object args)
        {
            KeyEventArgs e = args as KeyEventArgs;
            if (e.Key == Key.Enter)
            {
                System.Windows.Window parent = null;

                if (e.OriginalSource is System.Windows.Controls.TextBox)
                {
                    System.Windows.Controls.TextBox sender = e.OriginalSource as System.Windows.Controls.TextBox;
                    parent = System.Windows.Window.GetWindow(sender);
                }
                else
                {
                    System.Windows.Controls.PasswordBox sender = e.OriginalSource as System.Windows.Controls.PasswordBox;
                    parent = System.Windows.Window.GetWindow(sender);
                }

                // If nothing has changed do nothing
                if (FNDestObjectStore != null && FNDestObjectStore.SymbolicName == _currentTemplate.SelectedDestObjectStore)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(_currentTemplate.DestinationUrl) && !string.IsNullOrEmpty(_currentTemplate.DestUserName) && !string.IsNullOrEmpty(_currentTemplate.DestPassword))
                {
                    GetDestObjectStores(parent);
                }
            }
        }

        private void SrcPasswordChanged(object args)
        {
            System.Windows.RoutedEventArgs e = args as System.Windows.RoutedEventArgs;
            System.Windows.Controls.PasswordBox sender = e.OriginalSource as System.Windows.Controls.PasswordBox;
            CurrentTemplate.SrcPassword = ((System.Windows.Controls.PasswordBox)sender).Password;
        }

        private void DestPasswordChanged(object args)
        {
            System.Windows.RoutedEventArgs e = args as System.Windows.RoutedEventArgs;
            System.Windows.Controls.PasswordBox sender = e.OriginalSource as System.Windows.Controls.PasswordBox;
            CurrentTemplate.DestPassword = ((System.Windows.Controls.PasswordBox)sender).Password;
        }

        #endregion


        #region Migragion

        /// <summary>
        /// Start Copy or Move the documents from the source to the destination 
        /// </summary>
        private void StartMigration(object args)
        {
            try
            {
                if (!ValidateDBConfig())
                {
                    Utilities.ShowWarningMessageBox(string.Format("The database configuration is not complete"));
                    return;
                }

                tzOffset = GetTimeZoneOffset();

                //Get the List of the Destination Props names from mapping
                List<string> DestinationPropsNames = _currentTemplate.PropertiesMapping.Select(c => c.DestinationProperty.SymbolicName).ToList();
                //Check the required properties on the destination class
                if (FNDestProperties.Any(c => c.IsValueRequired == true && !DestinationPropsNames.Contains(c.SymbolicName)))
                {
                    Utilities.ShowWarningMessageBox(string.Format("There are some required properties on the destination class not mapped."));
                }
                else
                {
                    //Set the counts 
                    TotalCount = 0;
                    SuccessCount = 0;
                    CurrentVersionCount = 0;
                    ChildCount = 0;
                    VersionCount = 0;
                    FailCount = 0;
                    AlreadyProcessedCount = 0;

                    //Start the background worker
                    StartWork(args);
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
            }
            finally
            {
                docMappings.Clear();
            }
        }

        private void StartWork(object args)
        {
            try
            {
                loadingForm = new LoadingForm("", "Document " + (_currentTemplate.IsMove == 0 ? "Copy" : "Move") + " in process...");

                System.Windows.Window owner = args as System.Windows.Window;
                if (owner != null)
                {
                    loadingForm.Owner = owner;
                }

                loadingForm.Loaded += (s, e) =>
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    loadingForm.CancelOperation += (ss, ee) =>
                    {
                        worker.CancelAsync();
                        Message = "Process canceled";
                    };

                    checkExceptionLog = false;
                    DateTime startTime = DateTime.Now;

                    IProgress<string> progress = new Progress<string>(data => loadingForm.Message = data);

                    worker.DoWork += (ss, workerArgs) => StartWorkProcess(progress);

                    worker.RunWorkerCompleted += (ss, workerArgs) =>
                    {
                        loadingForm.Close();
                        DateTime stopTime = DateTime.Now;

                        recordAudit.SaveAuditTable();

                        Views.ProcessReport report = new Views.ProcessReport(CurrentVersionCount, VersionCount, ChildCount, AlreadyProcessedCount, FailCount, startTime, stopTime, checkExceptionLog);
                        if (owner != null)
                        {
                            report.Owner = owner;
                        }
                        else
                        {
                            report.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                        }
                        report.Show();

                        procChildDocs.Clear();
                        destChildDocs.Clear();
                        destVersions.Clear();
                        docAudit.Clear();
                    };

                    worker.RunWorkerAsync();
                };

                loadingForm.ShowDialog();
            }
            catch (Exception ex)
            {
                Utilities.ShowExceptionMessageBox(ex);
                if (loadingForm != null) { loadingForm.Close(); }
                recordAudit.SaveAuditTable();
            }
        }

        List<string> procChildDocs = new List<string>();
        List<DocumentMapping> docMappings = new List<DocumentMapping>();
        List<IDocument> destVersions = new List<IDocument>();
        List<AuditItem> docAudit = new List<AuditItem>();

        private void StartWorkProcess(IProgress<string> progress)
        {
            List<DocumentSearchItem> searchResults = FNSrcObj.SearchDocuments(FNSrcObjectStore, FNSrcObj.CreateSearchString(_currentTemplate, FNSrcCLSObj.SymbolicName, tzOffset));

            if (!isFullProduct)
            {
                searchResults = searchResults.Take(maxDemoCount).ToList();
            }

            if (searchResults == null || searchResults.Count() < 1)
            {
                Utilities.ShowWarningMessageBox("There are no documents on the source class.");
                return;
            }
            else
            {
                TotalCount = searchResults.Count();

                Message = string.Format("Source Current Version Document Count: {0}\r\n*This inludes parent and child documents", TotalCount);

                //Get the process settings
                if (_currentTemplate.IsMove == 1)
                {
                    string tip = IsInterObjectStoreMove() ? "Changing class for selected documents." : "New GUIDs will be created.";

                    if (Utilities.ShowDialogBox(string.Format(TotalCount + " current version documents to be processed.\nAre you sure, Do you want to move the documents?\n\nTip: " + tip)) == false)
                        return;
                }
                else
                {
                    if (Utilities.ShowDialogBox(string.Format(TotalCount + " current version documents to be processed.\n\nAre you sure, Do you want to start copy the documents?\n\nTip: New GUIDs will be created.")) == false)
                        return;
                }

                string connString = Utilities.GetDBConnectionString(dbConfig);
                docMappings.Clear();

                //Loop of the selected documents and start to Move or Copy it
                for (int i = 0; i < TotalCount; i++)
                {
                    //Update the UI status
                    progress.Report(string.Format("{0} of {1} documents", i + 1, TotalCount));

                    //Get the document ID
                    string SrcDocID = searchResults.ElementAt(i).Id;

                    //Check if the document already processed
                    if (docMappings.FirstOrDefault(x => x.SourceId == SrcDocID) == null) // Check if I've processed it first.
                    {
                        if (recordAudit.CheckDocumentIsProcessed(SrcDocID))
                        {
                            AlreadyProcessedCount++;
                        }
                        else
                        {
                            //Process the documentet
                            ProcessDocument(SrcDocID, connString);
                        }
                    }
                }

                //Display the completed result message
                Message = "Completed";
            }
        }

        /// <summary>
        /// Process the document Copy or Move
        /// </summary>
        /// <param name="SrcDocID"></param>
        private void ProcessDocument(string SrcDocID, string connString)
        {
            IDocument DestDoc = null;
            string SrcDocVSID = string.Empty;

            try
            {
                string FirstVersionID = string.Empty;

                FNSrcObj.EstablishConnection(false);

                //Get the source document
                IDocument SrcDoc = Factory.Document.FetchInstance(FNSrcObjectStore, new FileNet.Api.Util.Id(SrcDocID), null);

                if (SrcDoc.IsLocked() == true || SrcDoc.IsReserved == true)
                {
                    throw new Exception("Error: Document is checked out or locked. Unable to process document.");
                }

                int count = 0;
                foreach (IDocument doc in SrcDoc.ParentDocuments) // Or by parent count
                {
                    count++;
                }

                if (count > 0)
                {
                    return;
                }
                
                IDocument SrcDocVersion;
                List<IDocument> SrcDocVersions = new List<IDocument>();

                //Check is All versions
                if (_currentTemplate.IsAllVersions == 1)
                {
                    //Get the source versions
                    foreach (IDocument item in SrcDoc.Versions)
                    {
                        SrcDocVersions.Add(item);
                    }

                    //Get the first version
                    SrcDocVersion = SrcDocVersions.OrderBy(c => c.DateCreated).FirstOrDefault();
                    SrcDocVSID = SrcDocVersion.VersionSeries.Id.ToString();

                    //Save the first version ID
                    FirstVersionID = SrcDocVersion.Id.ToString();
                }
                else
                {
                    //Get the current version
                    SrcDocVersion = (IDocument)SrcDoc.CurrentVersion;
                    SrcDocVSID = SrcDocVersion.VersionSeries.Id.ToString();

                    //Save the first version ID
                    FirstVersionID = SrcDocVersion.Id.ToString();
                }

                // If we are just changing classes, there is no need to copy or delete anything.
                if (_currentTemplate.IsMove == 1 && IsInterObjectStoreMove())
                {
                    DocumentInterObjectStoreMove(SrcDocID, SrcDoc, SrcDocVersions, _currentTemplate.IsAllVersions == 1, _currentTemplate.PropertiesMapping.ToList(), FNDestCLSObj, connString);
                }
                else
                {
                    DocumentCopyOrMove(SrcDocID, SrcDoc, SrcDocVersion, SrcDocVersions, DestDoc, FirstVersionID, _currentTemplate.IsAllVersions == 1, _currentTemplate.PropertiesMapping.ToList(), FNDestCLSObj, FNSrcCLSObj, FNDestObjectStore, _currentTemplate.IsMove == 1, IsInterObjectStoreMove(), connString);
                }
            }
            catch (EngineRuntimeException ere)
            {
                //Audit the document
                recordAudit.AuditDocument(SrcDocID, SrcDocVSID, false, ere.ToString(), "", "", ere.Message);
                FailCount++;
            }
            catch (Exception ex)
            {
                //Audit the document
                recordAudit.AuditDocument(SrcDocID, SrcDocVSID, false, ex.ToString(), "", "", ex.Message);
                FailCount++;
            }
        }

        /// <summary>
        /// Inter object store move
        /// </summary>
        /// <param name="SrcDocID"></param>
        public void DocumentInterObjectStoreMove(string SrcDocID, IDocument SrcDoc, List<IDocument> SrcDocVersions, bool isAllVersions, List<PropertiesMap> propertiesMapping, FNClassItem desClsObj, string connString)
        {
            int count = 0;

            if (isAllVersions)
            {
                docAudit.Clear();

                foreach (var srcDoc in SrcDocVersions.OrderBy(c => c.DateCreated))
                {
                    srcDoc.ChangeClass(desClsObj.SymbolicName);

                    // Set the properties
                    MapProperties(propertiesMapping, srcDoc, srcDoc);

                    srcDoc.Save(FileNet.Api.Constants.RefreshMode.NO_REFRESH);
                    // Audit the document
                    docAudit.Add(new AuditItem { DestId = srcDoc.Id.ToString(), DestVersionId = srcDoc.VersionSeries.Id.ToString(), SourceId = srcDoc.Id.ToString(), SourceVersionId = srcDoc.VersionSeries.Id.ToString() });
                    count++;
                }

                VersionCount += (count - 1);
                CurrentVersionCount++;

                // Save the audits on success
                foreach (var item in docAudit)
                {
                    string id = item.SourceId;
                    try
                    {
                        recordAudit.AuditDocument(item.SourceId, item.SourceVersionId, true, string.Empty, item.DestId, item.DestVersionId, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        checkExceptionLog = true;
                        Utilities.WriteToExceptionLog(ex, "Unable to save audit log for document " + id);
                    }
                }
            }
            else
            {
                SrcDoc.ChangeClass(desClsObj.SymbolicName);

                // Set the properties
                MapProperties(propertiesMapping, SrcDoc, SrcDoc);

                SrcDoc.Save(FileNet.Api.Constants.RefreshMode.NO_REFRESH);

                // Audit the document
                recordAudit.AuditDocument(SrcDocID, SrcDoc.VersionSeries.Id.ToString(), true, string.Empty, SrcDocID, SrcDoc.VersionSeries.Id.ToString(), string.Empty);
                CurrentVersionCount++;
            }
        }

        private string docCurrVerId = "";
        private List<string> destChildDocs = new List<string>();

        /// <summary>
        /// Document copy or move
        /// </summary>
        /// <param name="SrcDocID"></param>
        public void DocumentCopyOrMove(string SrcDocID, IDocument SrcDoc, IDocument SrcDocVersion, List<IDocument> SrcDocVersions, IDocument DestDoc, string FirstVersionID, bool isAllVersions, List<PropertiesMap> propertiesMapping, FNClassItem desClsObj, FNClassItem srcClsObj, IObjectStore destObjStore, bool isMove, bool isInterObjectStoreMove, string connString)
        {
            FNSrcObj.EstablishConnection(false);
            string srcVersSeries = SrcDoc.VersionSeries.Id.ToString();
            string destVersSeries = null;
            string origDestId = "";

            procChildDocs.Clear();
            destChildDocs.Clear();
            destVersions.Clear();
            docAudit.Clear();

            try
            {
                bool hasChildren = SrcDocVersion.CompoundDocumentState == CompoundDocumentState.COMPOUND_DOCUMENT;

                foreach (IDocument doc in SrcDocVersion.ChildDocuments) // If there's any child docs then this doc has to be compound.
                {
                    hasChildren = true;
                }

                // Add the first version
                // Prepare the destination document 
                FNDestObj.EstablishConnection(false);
                DestDoc = Factory.Document.CreateInstance(destObjStore, desClsObj.SymbolicName);

                if (hasChildren) { DestDoc.CompoundDocumentState = CompoundDocumentState.COMPOUND_DOCUMENT; }

                CopySourceDocToDestDoc(SrcDocVersion, DestDoc, destObjStore, propertiesMapping);

                if (SrcDoc.MajorVersionNumber != null && SrcDoc.MajorVersionNumber > 0)
                {
                    DestDoc.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MAJOR_VERSION);
                }
                else
                {
                    DestDoc.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MINOR_VERSION);
                }

                DestDoc.Save(FileNet.Api.Constants.RefreshMode.REFRESH);

                docCurrVerId = DestDoc.Id.ToString();

                // Audit the parent document
                destVersSeries = DestDoc.VersionSeries.Id.ToString();
                docAudit.Add(new AuditItem { DestId = DestDoc.Id.ToString(), DestVersionId = destVersSeries, SourceId = FirstVersionID, SourceVersionId = SrcDocVersion.VersionSeries.Id.ToString() });
                docMappings.Add(new DocumentMapping { DestinationId = DestDoc.Id.ToString(), SourceId = FirstVersionID });
                destVersions.Add(DestDoc);

                origDestId = DestDoc.Id.ToString();

                if (_currentTemplate.AutoCreateFolders) // Add doc to folders if requested.
                {
                    FNSrcObj.EstablishConnection(false);
                    List<IFolder> folders = GetFolders(SrcDoc.FoldersFiledIn);
                    FNDestObj.EstablishConnection(false);
                    foreach (IFolder folder in folders)
                    {
                        IDynamicReferentialContainmentRelationship drcr = (IDynamicReferentialContainmentRelationship)folder.File(DestDoc, AutoUniqueName.AUTO_UNIQUE, null, DefineSecurityParentage.DEFINE_SECURITY_PARENTAGE);
                        drcr.Save(RefreshMode.NO_REFRESH);
                    }
                }

                int versionCount = 0;

                // Add the other versions
                if (isAllVersions)
                {
                    versionCount = ProcessDocumentVersions(DestDoc, SrcDocVersions, FirstVersionID, propertiesMapping, destObjStore, desClsObj);
                }

                // Process child documents
                int childCount = ProcessChildDocuments(SrcDocVersions, propertiesMapping, destObjStore, desClsObj);

                // If no errors, update the counts
                CurrentVersionCount++;
                VersionCount += versionCount;
                ChildCount += childCount;

                // Save the audits on success
                foreach (var item in docAudit)
                {
                    string id = item.SourceId;
                    try
                    {
                        recordAudit.AuditDocument(item.SourceId, item.SourceVersionId, true, string.Empty, item.DestId, item.DestVersionId, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        checkExceptionLog = true;
                        Utilities.WriteToExceptionLog(ex, "Unable to save audit log for document " + id);
                    }
                }

                // We've made it here, so no errors. Check if is move when done with doc processing.
                if (isMove && !isInterObjectStoreMove)
                {
                    // Delete the source document
                    FNSrcObj.EstablishConnection(false);

                    SrcDoc.Delete();
                    SrcDoc.Save(FileNet.Api.Constants.RefreshMode.NO_REFRESH);
                    SrcDoc = null;
                }
            }
            catch (Exception ex)
            {
                if (!IsInterObjectStoreMove() && !string.IsNullOrEmpty(docCurrVerId)) // Delete the document, versions and all children
                {
                    try
                    {
                        FNDestObj.EstablishConnection(false);
                        IDocument delDoc = Factory.Document.FetchInstance(FNDestObjectStore, new FileNet.Api.Util.Id(docCurrVerId), null);

                        // Iterate through component relationships. Set cascade delete.
                        foreach (IComponentRelationship cr in delDoc.ChildRelationships)
                        {
                            cr.ComponentCascadeDelete = ComponentCascadeDeleteAction.CASCADE_DELETE;
                            cr.Save(RefreshMode.REFRESH);
                        }

                        foreach (IDocument cdoc in delDoc.ChildDocuments) // Delete any child docs that did not get added to the parent. Should only ever be one.
                        {
                            if (!destChildDocs.Contains(cdoc.Id.ToString()))
                            {
                                try
                                {
                                    IDocument delChDoc = Factory.Document.FetchInstance(FNDestObjectStore, new FileNet.Api.Util.Id(cdoc.Id.ToString()), null);
                                    delChDoc.Delete();
                                    delChDoc.Save(RefreshMode.NO_REFRESH);
                                }
                                catch
                                {
                                    // Just keep moving
                                }
                            }
                        }

                        foreach (IDocument dver in delDoc.Versions)
                        {
                            if (dver.Id.ToString() != docCurrVerId)
                            {
                                dver.Delete();
                                dver.Save(RefreshMode.REFRESH);
                            }
                        }

                        delDoc.Delete();
                        delDoc.Save(FileNet.Api.Constants.RefreshMode.NO_REFRESH);
                    }
                    catch (Exception)
                    {
                        // If document delete fails, keep moving with document copy.
                    }
                }

                throw ex;
            }
        }

        /// <summary>
        /// Processes the document versions.
        /// </summary>
        /// <param name="DestDoc">The destination document.</param>
        /// <param name="SrcDocVersions">The source document versions.</param>
        /// <param name="FirstVersionID">The first version identifier.</param>
        /// <param name="propertiesMapping">The properties mappings.</param>
        /// <param name="destObjStore">The destination object store.</param>
        /// <param name="destFolders">The destination folders.</param>
        private int ProcessDocumentVersions(IDocument DestDoc, List<IDocument> SrcDocVersions, string FirstVersionID, List<PropertiesMap> propertiesMapping, IObjectStore destObjStore, FNClassItem desClsObj)
        {
            int count = 0;

            // Loop of the versions
            foreach (IDocument SrcVersionObj in SrcDocVersions.OrderBy(c => c.DateCreated))
            {
                FNSrcObj.EstablishConnection(false);

                if (SrcVersionObj.IsLocked() == true || SrcVersionObj.IsReserved == true)
                {
                    throw new Exception("Error: Document is checked out or locked. Unable to process document.");
                }

                // Check if not the current version
                if (SrcVersionObj.Id.ToString() != FirstVersionID)
                {
                    string srcId = SrcVersionObj.Id.ToString();
                    string srcVersId = SrcVersionObj.VersionSeries.Id.ToString();

                    bool hasChildren = SrcVersionObj.CompoundDocumentState == CompoundDocumentState.COMPOUND_DOCUMENT;

                    foreach (IDocument doc in SrcVersionObj.ChildDocuments) // If there's any child docs then this doc has to be compound.
                    {
                        hasChildren = true;
                    }

                    FNDestObj.EstablishConnection(false);
                    // Check out the VersionSeries object and save it.
                    IVersionSeries verSeries = DestDoc.VersionSeries;
                    verSeries.Checkout(FileNet.Api.Constants.ReservationType.COLLABORATIVE, null, null, null);
                    verSeries.Save(FileNet.Api.Constants.RefreshMode.REFRESH);
                    IDocument DestCurrentVer = (IDocument)verSeries.Reservation;

                    if (hasChildren) { DestCurrentVer.CompoundDocumentState = CompoundDocumentState.COMPOUND_DOCUMENT; }

                    CopySourceDocToDestDoc(SrcVersionObj, DestCurrentVer, destObjStore, propertiesMapping);

                    if (SrcVersionObj.MinorVersionNumber == null || SrcVersionObj.MinorVersionNumber == 0) // Check to see if it's a major or minor version update
                    {
                        DestCurrentVer.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MAJOR_VERSION);
                    }
                    else
                    {
                        DestCurrentVer.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MINOR_VERSION);
                    }

                    DestCurrentVer.Save(FileNet.Api.Constants.RefreshMode.REFRESH);

                    docCurrVerId = DestCurrentVer.Id.ToString();

                    docAudit.Add(new AuditItem { DestId = DestCurrentVer.Id.ToString(), DestVersionId = DestCurrentVer.VersionSeries.Id.ToString(), SourceId = srcId, SourceVersionId = srcVersId });
                    docMappings.Add(new DocumentMapping { DestinationId = DestCurrentVer.Id.ToString(), SourceId = srcId });
                    destVersions.Add(DestCurrentVer);
                    DestDoc = DestCurrentVer;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Processes the child documents.
        /// </summary>
        /// <param name="SrcDoc">The source document.</param>
        /// <param name="DestDoc">The destination document.</param>
        /// <param name="propertiesMapping">The properties mappings.</param>
        /// <param name="destObjStore">The destination object store.</param>
        /// <param name="desClsObj">The destination class object.</param>
        /// <param name="destFolders">The destination folders.</param>
        private int ProcessChildDocuments(List<IDocument> SrcDocVersions, List<PropertiesMap> propertiesMapping, IObjectStore destObjStore, FNClassItem desClsObj)
        {
            int count = 0;

            foreach (IDocument SrcDoc in SrcDocVersions.OrderBy(c => c.DateCreated))
            {
                List<IDocument> srcChildDocs = new List<IDocument>();

                FNSrcObj.EstablishConnection(false);

                foreach (IDocument cdoc in SrcDoc.ChildDocuments)
                {
                    srcChildDocs.Add(cdoc);
                }

                // We're not taking into account if the child doc has any child docs. Only parent-child, not parent-child-etc...
                // So child docs are not set to compound because of this.
                foreach (IDocument doc in srcChildDocs.OrderBy(x => x.DateCreated))
                {
                    if (doc.IsLocked() == true || doc.IsReserved == true)
                    {
                        throw new Exception("Error: Document is checked out or locked. Unable to process document.");
                    }

                    List<IDocument> childVersions = new List<IDocument>();

                    foreach (IDocument chdoc in doc.Versions)
                    {
                        childVersions.Add(chdoc);
                    }

                    childVersions = childVersions.OrderBy(x => x.DateCreated).ToList();

                    string firstVersionId = childVersions.First().Id.ToString();
                    IDocument srcChDoc = childVersions.First();
                    string srcVersId = srcChDoc.VersionSeries.Id.ToString();
                    IDocument childDoc = null;

                    if (!procChildDocs.Contains(srcChDoc.Id.ToString()))
                    {
                        procChildDocs.Add(srcChDoc.Id.ToString());

                        FNDestObj.EstablishConnection(false);
                        childDoc = Factory.Document.CreateInstance(destObjStore, desClsObj.SymbolicName);
                        CopySourceDocToDestDoc(doc, childDoc, destObjStore, propertiesMapping);

                        if (doc.MinorVersionNumber == null || doc.MinorVersionNumber == 0)
                        {
                            childDoc.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MAJOR_VERSION);
                        }
                        else
                        {
                            childDoc.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MINOR_VERSION);
                        }

                        childDoc.Save(FileNet.Api.Constants.RefreshMode.REFRESH);

                        docMappings.Add(new DocumentMapping { DestinationId = childDoc.Id.ToString(), SourceId = firstVersionId });
                        destChildDocs.Add(childDoc.Id.ToString());

                        if (_currentTemplate.AutoCreateFolders) // Add doc to folders if requested.
                        {
                            FNSrcObj.EstablishConnection(false);
                            List<IFolder> folders = GetFolders(doc.FoldersFiledIn);
                            FNDestObj.EstablishConnection(false);
                            foreach (IFolder folder in folders)
                            {
                                IDynamicReferentialContainmentRelationship drcr = (IDynamicReferentialContainmentRelationship)folder.File(childDoc, AutoUniqueName.AUTO_UNIQUE, null, DefineSecurityParentage.DEFINE_SECURITY_PARENTAGE);
                                drcr.Save(RefreshMode.NO_REFRESH);
                            }
                        }

                        FNSrcObj.EstablishConnection(false);
                        List<IComponentRelationship> icr = new List<IComponentRelationship>();
                        foreach (IComponentRelationship item in doc.ParentRelationships) // Establish the relationship types: Dynamic or Static
                        {
                            icr.Add(item);
                        }

                        foreach (IComponentRelationship item in icr)
                        {
                            IComponentRelationship cr = Factory.ComponentRelationship.CreateInstance(destObjStore, null);

                            if (item.ComponentRelationshipType == ComponentRelationshipType.DYNAMIC_CR)
                            {
                                cr.ComponentRelationshipType = ComponentRelationshipType.DYNAMIC_CR;
                                cr.VersionBindType = VersionBindType.LATEST_MAJOR_VERSION;
                            }
                            else
                            {
                                cr.ComponentRelationshipType = ComponentRelationshipType.STATIC_CR;
                            }

                            FNSrcObj.EstablishConnection(false);
                            string parentId = docMappings.First(x => x.SourceId == item.ParentComponent.Id.ToString()).DestinationId;
                            FNDestObj.EstablishConnection(false);
                            cr.ParentComponent = destVersions.First(x => x.Id.ToString() == parentId);
                            cr.ChildComponent = childDoc;
                            cr.Save(RefreshMode.REFRESH);
                        }

                        docAudit.Add(new AuditItem { DestId = childDoc.Id.ToString(), DestVersionId = childDoc.VersionSeries.Id.ToString(), SourceId = firstVersionId, SourceVersionId = srcVersId });
                        count++;
                    }

                    // Check for child doc versions
                    foreach (IDocument cDoc in childVersions)
                    {
                        if (!procChildDocs.Contains(cDoc.Id.ToString()))
                        {
                            procChildDocs.Add(cDoc.Id.ToString());
                            FNSrcObj.EstablishConnection(false);
                            string chldId = cDoc.Id.ToString();
                            string chldVersId = cDoc.VersionSeries.Id.ToString();

                            FNDestObj.EstablishConnection(false);
                            // Check out the VersionSeries object and save it.
                            IVersionSeries verSeries = childDoc.VersionSeries;
                            verSeries.Checkout(FileNet.Api.Constants.ReservationType.COLLABORATIVE, null, null, null);
                            verSeries.Save(FileNet.Api.Constants.RefreshMode.REFRESH);
                            IDocument DestCurrentVer = (IDocument)verSeries.Reservation;

                            CopySourceDocToDestDoc(cDoc, DestCurrentVer, destObjStore, propertiesMapping);

                            if (cDoc.MinorVersionNumber == null || cDoc.MinorVersionNumber == 0) // Check to see if it's a major or minor version update
                            {
                                DestCurrentVer.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MAJOR_VERSION);
                            }
                            else
                            {
                                DestCurrentVer.Checkin(FileNet.Api.Constants.AutoClassify.DO_NOT_AUTO_CLASSIFY, FileNet.Api.Constants.CheckinType.MINOR_VERSION);
                            }

                            DestCurrentVer.Save(FileNet.Api.Constants.RefreshMode.REFRESH);

                            docMappings.Add(new DocumentMapping { DestinationId = childDoc.Id.ToString(), SourceId = chldId });
                            destChildDocs.Add(childDoc.Id.ToString());
                            docAudit.Add(new AuditItem { DestId = DestCurrentVer.Id.ToString(), DestVersionId = DestCurrentVer.VersionSeries.Id.ToString(), SourceId = chldId, SourceVersionId = chldVersId });

                            childDoc = DestCurrentVer;
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the selected folder.
        /// </summary>
        /// <param name="SrcDoc">The source document.</param>
        /// <param name="destFolders">The destination folders.</param>
        /// <returns></returns>
        private List<IFolder> GetFolders(IFolderSet srcFolders)
        {
            IFolder destFolder = null;
            List<IFolder> folders = new List<IFolder>();

            foreach (IFolder folder in srcFolders)
            {
                folders.Add(folder);
            }

            List<IFolder> temp = new List<IFolder>();

            FNDestObj.EstablishConnection(false);
            foreach (var folder in folders)
            {
                // Check if each folder exists first. 
                try
                {
                    destFolder = Factory.Folder.FetchInstance(FNDestObjectStore, folder.PathName, null);
                }
                catch (EngineRuntimeException ex)
                {
                    // It couldn't find the folder...Dont know why it just doesnt return null...It should only reach here once for each new folder.
                }

                if (destFolder == null) // Create folder with parents (if necessary) if it does not exist.
                {
                    destFolder = CreateFolder(folder);
                }

                temp.Add(destFolder);
            }

            return temp;
        }

        /// <summary>
        /// Creates the selected folder.
        /// </summary>
        /// <param name="srcFolder">The source folder.</param>
        /// <param name="destFolders">The destination folders.</param>
        /// <returns></returns>
        private IFolder CreateFolder(IFolder srcFolder)
        {
            // Get the separated list of folders
            List<string> fldrList = srcFolder.PathName.Split('/').ToList();
            fldrList.RemoveAt(0); // Remove root folder

            FNDestObj.EstablishConnection(false);
            if (fldrList.Count() > 1) // Sub folders?
            {
                // Create each folder path from first to last folder
                List<string> paths = new List<string>();
                string path = "";
                foreach (var folder in fldrList)
                {
                    paths.Add(path += "/" + folder);
                }

                IFolder tempfolder = null;

                // Check if each folder exists in the destination and create if not
                for (int i = 0; i < paths.Count; i++)
                {
                    IFolder temp = null;

                    try
                    {
                        temp = Factory.Folder.FetchInstance(FNDestObjectStore, paths.ElementAt(i), null);
                    }
                    catch (EngineRuntimeException ex)
                    {
                        // Once again, I hate that it just desn't return null instead of an error
                    }

                    if (temp == null)
                    {
                        temp = Factory.Folder.CreateInstance(FNDestObjectStore, null);

                        if (tempfolder != null) // Add a parent if its not root.
                        {
                            temp.Parent = tempfolder;
                        }
                        else // Its root.
                        {
                            temp.Parent = FNDestObjectStore.RootFolder;
                        }

                        temp.FolderName = fldrList[i];
                        temp.Save(FileNet.Api.Constants.RefreshMode.REFRESH);

                        tempfolder = temp;
                    }
                    else
                    {
                        tempfolder = temp;
                    }
                }

                return tempfolder; // Return the target folder for the document
            }
            else // Just the one folder under the root folder. No sub folders.
            {
                IFolder tmpFldr = Factory.Folder.CreateInstance(FNDestObjectStore, null);
                tmpFldr.Parent = FNDestObjectStore.RootFolder;
                tmpFldr.FolderName = fldrList[0];
                tmpFldr.Save(FileNet.Api.Constants.RefreshMode.REFRESH);

                return tmpFldr; // Return the target folder for the document
            }
        }

        /// <summary>
        /// Copies the source document to destination document.
        /// </summary>
        /// <param name="SrcDocVersion">The source document version.</param>
        /// <param name="DestCurrentVer">The destination current version.</param>
        /// <param name="destObjStore">The destination object store.</param>
        /// <param name="propertiesMapping">The properties mappings.</param>
        /// <param name="destFolders">The destination folders.</param>
        private void CopySourceDocToDestDoc(IDocument SrcDocVersion, IDocument DestCurrentVer, IObjectStore destObjStore, List<PropertiesMap> propertiesMapping)
        {
            FNSrcObj.EstablishConnection(false);

            // Set the properties
            MapProperties(propertiesMapping, SrcDocVersion, DestCurrentVer);

            DestCurrentVer.MimeType = SrcDocVersion.MimeType;

            FNSrcObj.EstablishConnection(false);
            if (SrcDocVersion.ContentElements[0] is IContentReference)
            {
                //External content
                //Set the destination content
                IContentReference SrcCT = (IContentReference)SrcDocVersion.ContentElements[0];
                //Create destination content list
                IContentReference DestCT = Factory.ContentReference.CreateInstance(destObjStore);
                //Set the Source from the destination 
                DestCT.ContentLocation = SrcCT.ContentLocation;
                DestCT.ContentType = SrcCT.ContentType;
                //Create new content list for the destination document
                FNDestObj.EstablishConnection(false);
                IContentElementList DestCEL = Factory.ContentElement.CreateList();
                DestCEL.Add(DestCT);
                DestCurrentVer.ContentElements = DestCEL;
            }
            else
            {
                //Set the destination content
                IContentTransfer SrcCT = (IContentTransfer)SrcDocVersion.ContentElements[0];
                //Create destination content list
                IContentTransfer DestCT = Factory.ContentTransfer.CreateInstance(destObjStore);
                //Set the Source from the destination 
                DestCT.RetrievalName = SrcCT.RetrievalName;
                DestCT.ContentType = SrcCT.ContentType;

                DestCT.SetCaptureSource(SrcCT.AccessContentStream());
                //Create new content list for the destination document
                FNDestObj.EstablishConnection(false);
                IContentElementList DestCEL = Factory.ContentElement.CreateList();
                DestCEL.Add(DestCT);
                DestCurrentVer.ContentElements = DestCEL;
            }
        }

        /// <summary>
        /// Maps the properties of a document.
        /// </summary>
        /// <param name="mappings">The mappings.</param>
        /// <param name="srcDoc">The source document.</param>
        /// <param name="destDoc">The destination document.</param>
        private void MapProperties(List<PropertiesMap> mappings, IDocument srcDoc, IDocument destDoc)
        {
            // Map the modified on and by properties to custom properties
            if (_currentTemplate.UseEditedFields)
            {
                if (srcDoc.DateLastModified != null && srcDoc.DateLastModified.HasValue)
                {
                    destDoc.Properties["EditedDate"] = srcDoc.DateLastModified;
                }

                if (!string.IsNullOrEmpty(srcDoc.LastModifier))
                {
                    destDoc.Properties["EditedBy"] = srcDoc.LastModifier;
                }
            }

            foreach (PropertiesMap PropMapItem in mappings)
            {
                //Set the destination value for the source value
                if (srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName] != null) // Dont copy if null
                {
                    if (PropMapItem.SourceProperty.Cardinality == PropMapItem.DestinationProperty.Cardinality)
                    {
                        destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName];
                    }
                    else if (PropMapItem.SourceProperty.Cardinality == FileNet.Api.Constants.Cardinality.SINGLE && PropMapItem.DestinationProperty.Cardinality == FileNet.Api.Constants.Cardinality.LIST)
                    {
                        switch (PropMapItem.DestinationProperty.DataType)
                        {
                            case FileNet.Api.Constants.TypeID.BINARY:
                                var binList = Factory.BinaryList.CreateList();
                                binList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = binList;
                                break;
                            case FileNet.Api.Constants.TypeID.BOOLEAN:
                                var blList = Factory.BooleanList.CreateList();
                                blList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = blList;
                                break;
                            case FileNet.Api.Constants.TypeID.DATE:
                                var dtList = Factory.DateTimeList.CreateList();
                                dtList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = dtList;
                                break;
                            case FileNet.Api.Constants.TypeID.DOUBLE:
                                var dblList = Factory.Float64List.CreateList();
                                dblList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = dblList;
                                break;
                            case FileNet.Api.Constants.TypeID.GUID:
                                var guidList = Factory.IdList.CreateList();
                                guidList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = guidList;
                                break;
                            case FileNet.Api.Constants.TypeID.LONG:
                                var longList = Factory.Integer32List.CreateList();
                                longList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = longList;
                                break;
                            case FileNet.Api.Constants.TypeID.STRING:
                                var strList = Factory.StringList.CreateList();
                                strList.Add(srcDoc.Properties[PropMapItem.SourceProperty.SymbolicName]);
                                destDoc.Properties[PropMapItem.DestinationProperty.SymbolicName] = strList;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private bool IsInterObjectStoreMove()
        {
            return _currentTemplate.SourceUrl == _currentTemplate.DestinationUrl && FNSrcObjectStore.SymbolicName == FNDestObjectStore.SymbolicName;
        }

        #endregion


        #region Events/Misc

        /// <summary>
        /// Validates the database configuration
        /// </summary>
        /// <returns></returns>
        private bool ValidateDBConfig()
        {
            bool noErrors = true;

            if (string.IsNullOrEmpty(dbConfig.DatabaseName)) { noErrors = false; }
            if (string.IsNullOrEmpty(dbConfig.SqlServerName)) { noErrors = false; }

            if (dbConfig.AuthenticationType == 1)
            {
                if (string.IsNullOrEmpty(dbConfig.UserName)) { noErrors = false; }
                if (string.IsNullOrEmpty(dbConfig.Password)) { noErrors = false; }
            }

            return noErrors;
        }

        // The two methods below are needed because I dont want to hardcode the timezone offset for the app. 
        // This will allow the app to adjust to any timezone in the US for the correct UTC offset. This is necessary
        // because of the time difference between UTC time (as datetimes are stored) and local server time.
        // This will allow an offset from 0 (12am or UTC) to 12 (12pm or -12). Any others might not work.

        /// <summary>
        /// Gets the timezone offset of the database or local if an error
        /// </summary>
        /// <returns></returns>
        private int GetTimeZoneOffset()
        {
            int offset = -(int.Parse(DateTime.Now.ToString("zz"))); // Local default
            return offset;
        }

        /// <summary>
        /// Formats the timezone offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private string FormatTimeZoneOffset(int offset)
        {
            if (offset >= 0 && offset <= 9) // If single digit
            {
                return "T0" + offset + "0000Z";
            }
            else // Double digits
            {
                return "T" + offset + "0000Z";
            }
        }

        public void SaveMtomUrls()
        {
            Utilities.SaveUrlList(MtomUrls);
        }

        public event SetPasswordEventHandler SetSrcPassword;

        private void OnSetSrcPassword(string password)
        {
            SetSrcPassword?.Invoke(this, new PasswordEventArgs(password));
        } 

        public event SetPasswordEventHandler SetDestPassword;

        private void OnSetDestPassword(string password)
        {
            SetDestPassword?.Invoke(this, new PasswordEventArgs(password));
        }

        public event SetMessageEventHandler SetMessage;

        private void OnSetMessage(string message)
        {
            SetMessage?.Invoke(this, new SetMessageEventArgs(message));
        }

        public event ExitApplicationEventHandler ExitApplication;

        private void OnExitApplication()
        {
            ExitApplication?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
