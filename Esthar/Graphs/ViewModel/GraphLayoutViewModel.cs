using System.ComponentModel;

namespace Esthar
{
    public class GraphLayoutViewModel : INotifyPropertyChanged
	{
		private bool isSelected;
		private string layoutAlgorithmType;
		private AsmGraph graph;

		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				if (value != isSelected)
				{
					isSelected = value;
					NotifyChanged("IsSelected");
				}
			}
		}

		public string LayoutAlgorithmType
		{
			get { return layoutAlgorithmType; }
			set
			{
				if (value != layoutAlgorithmType)
				{
					layoutAlgorithmType = value;
					NotifyChanged("LayoutAlgorithmType");
				}
			}
		}

		public AsmGraph Graph
		{
			get { return graph; }
			set
			{
				if (value != graph)
				{
					graph = value;
					NotifyChanged("Graph");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}