using Game;
using System;
using System.Xml.Linq;

public class EditElectricDialog : Dialog
{
	public Action<int> m_handler;

	public SliderWidget m_delaySlider;

	public ButtonWidget m_plusButton;

	public ButtonWidget m_minusButton;

	public LabelWidget m_delayLabel;

	public ButtonWidget m_okButton;

	public ButtonWidget m_cancelButton;

	public float m_delay;

	public EditElectricDialog(float delay)
	{
		XElement node = ContentManager.Get<XElement>("Dialogs/EditAdjustableDelayGateDialog");
		WidgetsManager.LoadWidgetContents(this, this, node);
		//  m_delaySlider = Children.Find<SliderWidget>("EditAdjustableDelayGateDialog.DelaySlider");
		// m_plusButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.PlusButton");
		// m_minusButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.MinusButton");
		m_delayLabel = Children.Find<LabelWidget>("EditAdjustableDelayGateDialog.Label");
		m_okButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.OK");
		m_cancelButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Cancel");

		m_delay = delay;
		UpdateControls();
	}

	public override void Update()
	{
		if (m_okButton.IsClicked)
		{
			//  Dismiss(m_delay);
		}
		if (Input.Cancel || m_cancelButton.IsClicked)
		{
			Dismiss(null);
		}
		UpdateControls();
	}

	public void UpdateControls()
	{
		//  m_delaySlider.Value = m_delay;
		// m_minusButton.IsEnabled = false;
		// m_plusButton.IsEnabled = false;
		m_delayLabel.Text = $"{(float)(m_delay) * 0.01f:0.00} Energy";
	}

	public void Dismiss(int? result)
	{
		DialogsManager.HideDialog(this);
		if (m_handler != null && result.HasValue)
		{
			m_handler(result.Value);
		}
	}
}