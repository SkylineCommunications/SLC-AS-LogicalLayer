﻿using LogicalLayer_1.ElementAlarmMonitor;
using LogicalLayer_1.Utils;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.CICD.Parsers.Protocol.Xml;
using Skyline.DataMiner.Core.DataMinerSystem.Automation;
using Skyline.DataMiner.Core.DataMinerSystem.Common;
using Skyline.DataMiner.Utils.InteractiveAutomationScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalLayer_1.ViewAlarmMonitor
{
    public class ViewAlarmMonitorView : Dialog
    {
        private readonly IEngine _engine;
        private readonly Label _viewAlarmMonitorName = new Label("View Alarm Monitor Name: ") { Width = 200 };
        private readonly Label _viewName = new Label("View Name: ") { Width = 200 };
        private readonly Label _parameterName = new Label("Parameter Name: ") { Width = 200 };
        private Protocol _protocol;
        private IDms _dms;

        public ViewAlarmMonitorView(IEngine engine) : base(engine)
        {
            _engine = engine;
            _dms = engine.GetDms();
            Title = "View Alarm Monitor";
            ViewAlarmMonitorName = new TextBox
            {
                Width = 200,
                Height = 20,
            };
            View = new DropDown
            {
                Width = 200,
            };
            Parameter = new DropDown
            {
                Width = 200,
            };
            Add = new Button("Add")
            {
                Width = 200,
            };
            Back = new Button("Back")
            {
                Width = 200,
            };
            Close = new Button("Close")
            {
                Width = 200,
            };
            Add.Pressed += (s, e) => OnAdd(s, e);
            Back.Pressed += Back_Pressed;
            View.SetOptions(LayoutDesigner.GetDropdownValuesWithSelect(_dms.GetViews().Select(x => x.Name).OrderBy(x => x)));
            View.Selected = LayoutDesigner.OptionSelected;
            View.Changed += View_Changed;
            Close.Pressed += (s, e) => OnClosePressed?.Invoke(this, EventArgs.Empty);
            SetupLayout();
        }

        public event EventHandler<ViewAlarmMonitorEventArgs> OnAddPressed;

        public event EventHandler OnBackPressed;

        public event EventHandler OnClosePressed;

        public TextBox ViewAlarmMonitorName { get; set; }

        public DropDown View { get; set; }

        public DropDown Parameter { get; set; }

        public TextBox Index { get; set; }

        public Button Add { get; set; }

        public Button Back { get; set; }

        public Button Close { get; set; }

        private void Back_Pressed(object sender, EventArgs e)
        {
            OnBackPressed?.Invoke(sender, e);
        }

        private void OnAdd(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ViewAlarmMonitorName.Text))
            {
                return;
            }

            if (View.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            if (Parameter.Selected == LayoutDesigner.OptionSelected)
            {
                return;
            }

            OnAddPressed?.Invoke(this, new ViewAlarmMonitorEventArgs
            {
                ViewAlarmMonitorName = ViewAlarmMonitorName.Text,
                View = _dms.GetViews().First(x => x.Name == View.Selected),
                ViewParameter = Parameter.Selected,
            });
        }

        private void View_Changed(object sender, DropDown.DropDownChangedEventArgs e)
        {
            List<string> options = new List<string>
            {
                "[View Alarm State]",
                LayoutDesigner.OptionSelected
            };
            Parameter.SetOptions(options.OrderBy(x => x));
            Parameter.Selected = LayoutDesigner.OptionSelected;
        }

        private void SetupLayout()
        {
            int rowNumber = 0;

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: rowNumber,
                orderedWidgets: new Widget[] { _viewAlarmMonitorName, ViewAlarmMonitorName });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _viewName, View });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { _parameterName, Parameter });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { Back, Add });

            LayoutDesigner.SetComponentsOnRow(
                dialog: this,
                row: ++rowNumber,
                orderedWidgets: new Widget[] { Close });
        }
    }
}