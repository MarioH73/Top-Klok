namespace TopKlok;

internal sealed class TimerSettingsForm : SettingsDialogForm
{
    private readonly NumericUpDown _workHours = CreateInput(24);
    private readonly NumericUpDown _workMinutes = CreateInput(59);
    private readonly NumericUpDown _workSeconds = CreateInput(59);
    private readonly NumericUpDown _pauseHours = CreateInput(24);
    private readonly NumericUpDown _pauseMinutes = CreateInput(59);
    private readonly NumericUpDown _pauseSeconds = CreateInput(59);

    public TimeSpan Duration { get; private set; }
    public TimeSpan PauseDuration { get; private set; }

    public TimerSettingsForm(
        string title,
        TimeSpan initialPauseDuration,
        string startButtonText) : base(title, new Size(340, 340))
    {
        SetDuration(_pauseHours, _pauseMinutes, _pauseSeconds, initialPauseDuration);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            ColumnCount = 1,
            RowCount = 3
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.Controls.Add(CreateDurationGroup(
            AppLanguage.Text("Werktijd", "Work duration"),
            _workHours, _workMinutes, _workSeconds), 0, 0);
        layout.Controls.Add(CreateDurationGroup(
            AppLanguage.Text("Pauzetijd", "Break duration"),
            _pauseHours, _pauseMinutes, _pauseSeconds), 0, 1);

        var startButton = new Button { Text = startButtonText, Dock = DockStyle.Fill };
        startButton.Click += (_, _) =>
        {
            Duration = GetDuration(_workHours, _workMinutes, _workSeconds);
            PauseDuration = GetDuration(_pauseHours, _pauseMinutes, _pauseSeconds);
            if (Duration <= TimeSpan.Zero || PauseDuration <= TimeSpan.Zero)
            {
                MessageBox.Show(this,
                    AppLanguage.Text(
                        "Stel zowel de werktijd als de pauzetijd groter dan nul in.",
                        "Enter work and break durations greater than zero."),
                    AppLanguage.Text("Ongeldige timer", "Invalid timer"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        };
        layout.Controls.Add(startButton, 0, 2);
        ContentArea.Controls.Add(layout);
        AcceptButton = startButton;
    }

    private static GroupBox CreateDurationGroup(
        string title,
        NumericUpDown hours,
        NumericUpDown minutes,
        NumericUpDown seconds)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            Padding = new Padding(8)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
        AddRow(layout, 0, AppLanguage.Text("Uren:", "Hours:"), hours);
        AddRow(layout, 1, AppLanguage.Text("Minuten:", "Minutes:"), minutes);
        AddRow(layout, 2, AppLanguage.Text("Seconden:", "Seconds:"), seconds);

        var group = new GroupBox { Text = title, Dock = DockStyle.Fill };
        group.Controls.Add(layout);
        return group;
    }

    private static NumericUpDown CreateInput(int maximum) => new()
    {
        Minimum = 0,
        Maximum = maximum,
        Dock = DockStyle.Fill
    };

    private static void SetDuration(
        NumericUpDown hours,
        NumericUpDown minutes,
        NumericUpDown seconds,
        TimeSpan duration)
    {
        hours.Value = Math.Min((decimal)Math.Floor(duration.TotalHours), hours.Maximum);
        minutes.Value = duration.Minutes;
        seconds.Value = duration.Seconds;
    }

    private static TimeSpan GetDuration(
        NumericUpDown hours,
        NumericUpDown minutes,
        NumericUpDown seconds) =>
        TimeSpan.FromHours((double)hours.Value) +
        TimeSpan.FromMinutes((double)minutes.Value) +
        TimeSpan.FromSeconds((double)seconds.Value);

    private static void AddRow(TableLayoutPanel layout, int row, string label, Control input)
    {
        layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        layout.Controls.Add(input, 1, row);
    }
}
