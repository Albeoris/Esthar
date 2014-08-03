using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Esthar.Data;
using Esthar.Data.Transform;

namespace Esthar
{
    /// <summary>
    /// Логика взаимодействия для AsmVertex.xaml
    /// </summary>
    public partial class AsmVertex
    {
        public readonly string Id;
        public Location Location { get; private set; }
        public AsmSegment Segment { get; private set; }
        public bool IsEmpty { get; private set; }

        public AsmVertex(Location location, AsmSegment segment)
        {
            InitializeComponent();

            Location = location;
            Segment = segment;
            Id = Segment.Event.Module.Title + ' ' + Segment.Event.Title + ' ' + Segment.Offset;

            FillPanel();
        }

        private void FillPanel()
        {
            AsmCommandFactory factory = new AsmCommandFactory(Segment);
            List<AsmCommand> commands = factory.FindAll(JsmCommand.MES, JsmCommand.AMES, JsmCommand.AMESW, JsmCommand.RAMESW, JsmCommand.ASK, JsmCommand.AASK);

            if (commands.Count > 0)
            {
                Panel.Children.Add(new TextBlock { Text = Id });
                IsEmpty = false;
                Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                IsEmpty = true;
                Visibility = System.Windows.Visibility.Collapsed;
            }

            foreach (AsmCommand command in commands)
            {
                MessageWindow window = MessageWindow.FromCommand(command);
                window.Message = Location.Monologues[window.MessageId];

                TextBlock control = new TextBlock { Text = window.DisplayName, Tag = window };
                control.MouseLeftButtonDown += OnMessageWindowClick;

                Panel.Children.Add(control);
            }
        }

        private void OnMessageWindowClick(object sender, MouseButtonEventArgs e)
        {
            MessageWindow messageWindow = (MessageWindow)((TextBlock)sender).Tag;
        }
    }
}