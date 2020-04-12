﻿using System;
using System.Windows;
using System.Windows.Controls;
using Communication.Commands;
using Model.MeasurementRoutine.GlobalVariables;

namespace Controller.MainWindow.MeasurementRoutine
{
    /// <summary>
    /// Holds data used to describe to users the usage of a special python variable that they can use in a python script
    /// </summary>
    public class VariableUsageDescriptor
    {
        /// <summary>
        /// All data types of variables
        /// </summary>
        public enum VariableTypeEnum { Integer, Boolean, String, StringArray, DoubleArray }
        /// <summary>
        /// All access types of variables
        /// </summary>
        public enum AccessTypeEnum { Read, Write, ReadWrite }

        /// <summary>
        /// Sets the type of the variable.
        /// </summary>
        /// <value>
        /// The type of the variable.
        /// </value>
        public VariableTypeEnum VariableType { set; private get; }

        /// <summary>
        /// Sets the type of the access.
        /// </summary>
        /// <value>
        /// The type of the access.
        /// </value>
        public AccessTypeEnum AccessType { set; private get; }

        /// <summary>
        /// Gets or sets the name of the variable.
        /// </summary>
        /// <value>
        /// The name of the variable.
        /// </value>
        public string VariableName { set; get; }

        /// <summary>
        /// Gets the variable type as string.
        /// </summary>
        /// <value>
        /// The variable type as string.
        /// </value>
        public string VariableTypeAsString
        {
            get
            {
                switch (VariableType)
                {
                    case VariableUsageDescriptor.VariableTypeEnum.Boolean:
                        return "Boolean";
                    case VariableUsageDescriptor.VariableTypeEnum.Integer:
                        return "Integer";
                    case VariableTypeEnum.String:
                        return "String";
                    case VariableTypeEnum.StringArray:
                        return "Array of Strings";
                    case VariableTypeEnum.DoubleArray:
                        return "List of Doubles";
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Gets the read or write.
        /// </summary>
        /// <value>
        /// The read or write.
        /// </value>
        public string ReadOrWrite
        {
            get
            {
                switch (AccessType)
                {
                    case AccessTypeEnum.Read:
                        return "Read Only";
                    case AccessTypeEnum.Write:
                        return "Write";
                    case AccessTypeEnum.ReadWrite:
                        return "Read/Write";
                }

                return "Unknown";
            }
        }
        public string Remarks { set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableUsageDescriptor"/> class.
        /// </summary>
        public VariableUsageDescriptor()
        {
            VariableType = VariableTypeEnum.Integer;
            AccessType = AccessTypeEnum.Read;
        }
    }

    public class MeasurementRoutineScriptController : BaseController
    {
        /// <summary>
        /// The possible outcomes of this dialog window.
        /// </summary>
        public enum SetScriptResult
        {
            CANCEL_OR_CLOSE,
            SAVE
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { set; get; }
        /// <summary>
        /// The initialization script text.
        /// </summary>
        private string initializationScript;
        /// <summary>
        /// The repetitive script text.
        /// </summary>
        private string script;

        private bool requiresCodeCheck;
        /// <summary>
        /// The command that is triggered when the save button is clicked.
        /// </summary>
        private RelayCommand _saveCommand;
        /// <summary>
        /// The command that is triggered when the close button is clicked.
        /// </summary>
        private RelayCommand _closeCommand;
        /// <summary>
        /// The command that shows information about the execution steps in the measurement routine
        /// </summary>
        private RelayCommand _showExecutionStepsInfo;
        /// <summary>
        /// The command that shows a window with sample scripts
        /// </summary>
        private RelayCommand _showSampleScriptCommand;

        private RelayCommand _checkCodeCommand;
        /// <summary>
        /// Gets or sets the initialization script.
        /// </summary>
        /// <value>
        /// The initialization script.
        /// </value>
        public string InitializationScript
        {
            get { return initializationScript; }
            set 
            { 
                initializationScript = value;
                this.requiresCodeCheck = true;
                OnPropertyChanged("InitializationScript");
            }
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public SetScriptResult Result
        {
            set;
            get;
        }
        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        /// <value>
        /// The script.
        /// </value>
        public string Script
        {
            get { return script; }
            set
            {
                script = value;
                this.requiresCodeCheck = true;
                OnPropertyChanged("Script");
            }
        }

        
        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <value>
        /// The save command.
        /// </value>
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(Save, CanSaveScripts);
                }
                return _saveCommand;
            }
        }
        /// <summary>
        /// Gets the close command.
        /// </summary>
        /// <value>
        /// The close command.
        /// </value>
        public RelayCommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(CloseWindow);

                return _closeCommand;
            }
        }

        public RelayCommand ShowExecutionStepsInfo
        {
            get
            {
                if (_showExecutionStepsInfo == null)
                    _showExecutionStepsInfo = new RelayCommand(ShowExecutionSteps);
                return _showExecutionStepsInfo;
            }
           

        }
        public RelayCommand ShowSampleScriptCommand
        {
            get
            {
                if (_showSampleScriptCommand == null)
                {
                    _showSampleScriptCommand = new RelayCommand(ShowSampleScript, CanShowSampleScript);
                }

                return _showSampleScriptCommand;
            }
        }
        public RelayCommand CheckCodeCommand {
            get
            {
                if (_checkCodeCommand == null)
                    _checkCodeCommand = new RelayCommand(CheckCode);

                return _checkCodeCommand;
            }
        }


        public  VariableUsageDescriptor[] BuiltInVariables { set; get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementRoutineScriptController"/> class.
        /// </summary>
        /// <param name="initializationScript">The initial script.</param>
        public MeasurementRoutineScriptController(string initializationScript, string repetitiveScript, bool isReadOnly)
        {
            this.requiresCodeCheck = true;
            this.script = repetitiveScript;
            this.initializationScript = initializationScript;
            this.IsReadOnly = isReadOnly;
            Result = SetScriptResult.CANCEL_OR_CLOSE;
            InitializeBuiltInVariables();
        }

        private void InitializeBuiltInVariables()
        {
            BuiltInVariables = new VariableUsageDescriptor[]{
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_CURRENT_MODE,
                    AccessType = VariableUsageDescriptor.AccessTypeEnum.ReadWrite,
                    Remarks = "The index of the current model (0 for the primary model, higher for secondary models). You can assign a value to this variable in order to affect the model of the next cycle."
                },
                new VariableUsageDescriptor(){
                    VariableName = GlobalVariableNames.ROUTINE_ARRAY,
                    AccessType = VariableUsageDescriptor.AccessTypeEnum.ReadWrite,
                    Remarks = String.Format(
                    "A list of double values accessible from python scripts outside the measurement routine (e.g., dynamic variables). Use \"{0}.Add(15)\" to add the value \"15\" at the end of the list (increases the size of the list). Use \"{0}[0]\" to read or write an existing element at the position 0 of the list."
                    , GlobalVariableNames.ROUTINE_ARRAY)
                    
                },
                new VariableUsageDescriptor(){
                    VariableName= MeasurementRoutineManager.VAR_PRIMARY_MODEL,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.String,
                    Remarks = "The path of the primary model."
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_SECONDARY_MODELS,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.StringArray,
                    Remarks = "An array of the paths of secondary models"
                },
                
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_PREVIOUS_MODE,
                    Remarks = "The index of the previous model (0 for the primary model, higher for secondary models)"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_START_ROUTINE,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.Boolean,
                    Remarks = "A boolean value indicating whether the current cycle is the first cycle in the routine"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_GLOBAL_COUNTER,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.Integer,
                    Remarks = "The current value of the global counter"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_NUMBER_OF_ITERATIONS,
                    Remarks = "The total number of iterations of the current model based on the iterator variables."
                },

                //Ebaa 29.05.2018 The name of next iteration should be last iteration
                 new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_NEXT_ITERATION,
                    Remarks = "The number of the previous iteration within the current scan (1 if not iterating)"
                //new VariableUsageDescriptor(){
                //    VariableName = MeasurementRoutineManager.VAR_NEXT_ITERATION,
                //    Remarks = "The number of the next iteration within the current scan (0 if not iterating)"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_COMPLETED_SCANS,
                    Remarks = "The number of scans completed in the current run. (0 if not iterating)"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_START_COUNTER_OF_SCANS,
                    Remarks = "The value of the global counter when the current set of scans started"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_SCAN_ONLY_ONCE,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.Boolean,
                    Remarks = "Indicates whether iterators will only scanned once or not."
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_CONTROL_LE_CROY,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.Boolean,
                    Remarks = "Indicates whether LeCroy will be controlled"
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_STOP_AFTER_SCAN,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.Boolean,
                    Remarks = "Indicates whether the output will be stopped after finishing one scan or not."
                },
                new VariableUsageDescriptor(){
                    VariableName = MeasurementRoutineManager.VAR_SHUFFLE_ITERATIONS,
                    VariableType = VariableUsageDescriptor.VariableTypeEnum.Boolean,
                    Remarks = "Indicates whether iterations are shuffled or not."
                }
                
            };
        }

        /// <summary>
        /// Executed when the save command is triggered
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void Save(object parameter)
        {
            Result = SetScriptResult.SAVE;
            CloseWindow(parameter);
        }

        private bool CanSaveScripts(object parameter)
        {
            return !IsReadOnly && !this.requiresCodeCheck;
        }

        private bool CanShowSampleScript(object parameter)
        {
            return !IsReadOnly;
        }

        /// <summary>
        /// Executed when the cancel command is executed.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void CloseWindow(object parameter)
        {
            if (parameter != null)
            {
                UserControl uc = (UserControl)parameter;
                Window w = Window.GetWindow(uc);
                w.Close();
            }
        }

        private void ShowSampleScript(object parameter)
        {
            MeasurementRoutineSampleScriptController sampleScriptController = new MeasurementRoutineSampleScriptController();
            Window w = WindowsHelper.CreateWindowToHostViewModel(sampleScriptController, true);
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.Title = "Sample Measurement Routine Scripts";
            w.ShowDialog();

            if (sampleScriptController.Result == MeasurementRoutineSampleScriptController.SampleScriptResult.COPY)
            {
                this.InitializationScript = sampleScriptController.SampleInitializationScript;
                this.Script = sampleScriptController.SampleRepetitiveScript;
            }

        }

        //Ebaa 29.05.2018
        private void ShowExecutionSteps(object parameter)
        {
            String stepsInfo="The main execution steps of the measurement routine are as follows:"+"\n"+
                "1- Execute the repetitive python script and decide which model to be loaded next."+"\n" +
                "2- Increase current model counters."+"\n" +
                "3- Load the next model to be executed."+ "\n"+ 
                "4- Save the next model to be executed to the database." +"\n"+
                "5- Execute the model that was loaded before.";
            MessageBox.Show(stepsInfo);
        }


        private void CheckCode(object parameter)
        {
            String result = "";
            if (Script == null || Script.Trim().Length == 0)
            {
                result = "You must specify a python Script!";
            }
            else
            {
                string errorMessage;
                string scriptToAnalyze = "";

                if (!String.IsNullOrEmpty(InitializationScript))
                    scriptToAnalyze = InitializationScript + "\n";

                scriptToAnalyze += Script;
                if (!MeasurementRoutineManager.ValidatePythonScript(scriptToAnalyze, out errorMessage, false))
                {
                    result = errorMessage;
                }
            }

            if (result.Length > 0)
            {
                MessageBox.Show(result, "Invalid Python Script", MessageBoxButton.OK, MessageBoxImage.Error);   
            }
            else
            {
                this.requiresCodeCheck = false;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                string result = "";

                switch (columnName)
                {
                    case "Script":
                        
                        break;

                    default:
                        break;
                }

                return result;
            }
        }
    }
}