namespace Game
{
	public class EditElectricDialog : EditAdjustableDelayGateDialog
	{
		public EditElectricDialog(int delay) : base(delay, null)
		{
			m_minusButton.IsEnabled = false;
			m_plusButton.IsEnabled = false;
			UpdateControls();
		}

		public override void Update()
		{
			if (m_okButton.IsClicked || Input.Cancel || m_cancelButton.IsClicked)
			{
				Dismiss(null);
			}
			UpdateControls();
		}

		public new void UpdateControls()
		{
			m_delayLabel.Text = $"{m_delay * 0.01f:0.00} Energy";
		}
	}
}