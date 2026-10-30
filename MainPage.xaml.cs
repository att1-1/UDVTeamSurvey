using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace UDVTeamSurvey;

public partial class MainPage : ContentPage
{
    private bool _isInitialized;
    private bool _isResetting;

    public MainPage()
    {
        // Один и тот же читаемый интерфейс при светлой и тёмной теме Windows.
        if (Application.Current != null)
            Application.Current.UserAppTheme = AppTheme.Light;

        InitializeComponent();
        _isInitialized = true;
        UpdateSubmitState();
    }

    private bool HasRequiredFields()
    {
        string name = _NameEntry_.Text?.Trim() ?? string.Empty;
        string email = _SecondEntry_.Text?.Trim() ?? string.Empty;

        int at = email.IndexOf('@');
        int dot = email.LastIndexOf('.');
        bool validEmail = at > 0 && dot > at + 1 && dot < email.Length - 1
                          && email.LastIndexOf('@') == at && !email.Contains(' ');

        return !string.IsNullOrWhiteSpace(name) && validEmail;
    }

    private void OnAnswerChanged()
    {
        if (!_isInitialized || _isResetting)
            return;

        UpdateSubmitState();
        HideOldResult();
    }

    private void OnRequiredFieldChanged(object? sender, TextChangedEventArgs e) => OnAnswerChanged();
    private void OnNotesChanged(object? sender, TextChangedEventArgs e) => OnAnswerChanged();
    private void OnChoiceChanged(object? sender, CheckedChangedEventArgs e) => OnAnswerChanged();
    private void OnSwitchChanged(object? sender, ToggledEventArgs e) => OnAnswerChanged();
    private void OnDateChanged(object? sender, DateChangedEventArgs e) => OnAnswerChanged();
    private void OnHoursChanged(object? sender, ValueChangedEventArgs e) => OnAnswerChanged();

    private void OnRoleChanged(object? sender, EventArgs e)
    {
        if (!_isInitialized || _isResetting)
            return;

        UpdateRoleText();
        OnAnswerChanged();
    }

    private void UpdateRoleText()
    {
        // В Windows текст выбранного элемента в стандартном Picker может не
        // отображаться в свернутом выпадающем списке. Показываем выбранную
        // роль в заголовке самого Picker, а не отдельной подписью под ним.
        bool selected = _RolePicker_.SelectedIndex >= 0;
        _RolePicker_.Title = selected
            ? _RolePicker_.Items[_RolePicker_.SelectedIndex]
            : "Выбери роль";
        _RolePicker_.TitleColor = selected
            ? Color.FromArgb("#17243D")
            : Color.FromArgb("#8190A5");
    }

    private void UpdateSubmitState()
    {
        bool ready = HasRequiredFields();
        _SubmitButton_.IsEnabled = ready;
        _ReadyLabel_.IsVisible = ready;
    }

    private void HideOldResult()
    {
        _ResultBorder_.IsVisible = false;
    }

    private string SelectedExperience()
    {
        if (_OptionOneRadio_.IsChecked) return "Опыта ещё не было";
        if (_OptionTwoRadio_.IsChecked) return "До 6 месяцев";
        if (_OptionThreeRadio_.IsChecked) return "6–12 месяцев";
        if (_OptionFourRadio_.IsChecked) return "От 1 до 2 лет";
        if (_OptionFiveRadio_.IsChecked) return "Более 2 лет";
        return "Не выбран";
    }

    private void OnShowResultClicked(object? sender, EventArgs e)
    {
        // Дополнительная проверка обязательных полей непосредственно при отправке.
        if (!HasRequiredFields())
        {
            UpdateSubmitState();
            return;
        }

        string role = _RolePicker_.SelectedIndex >= 0
            ? _RolePicker_.Items[_RolePicker_.SelectedIndex]
            : "Не выбрана";

        var interests = new List<string>();
        if (_ExtraOneCheck_.IsChecked) interests.Add("Backend-разработка");
        if (_ExtraTwoCheck_.IsChecked) interests.Add("Frontend-разработка");
        if (_ExtraThreeCheck_.IsChecked) interests.Add("Мобильная разработка");
        if (_ExtraFourCheck_.IsChecked) interests.Add("Тестирование и обеспечение качества");
        if (_ExtraFiveCheck_.IsChecked) interests.Add("Аналитика и работа с данными");
        if (_ExtraSixCheck_.IsChecked) interests.Add("UI/UX-дизайн");
        if (_ExtraSevenCheck_.IsChecked) interests.Add("Управление IT-проектами");

        _ResultName_Value_.Text = _NameEntry_.Text?.Trim() ?? "";
        _ResultEmail_Value_.Text = _SecondEntry_.Text?.Trim() ?? "";
        _ResultRole_Value_.Text = role;
        _ResultExperience_Value_.Text = SelectedExperience();
        _ResultInterests_Value_.Text = interests.Count > 0
            ? string.Join(", ", interests)
            : "Не выбраны";
        _ResultMeeting_Value_.Text = _FlexibleSwitch_.IsToggled ? "Готов" : "Не готов";
        _ResultStartDate_Value_.Text = _StartDatePicker_.Date?.ToString("dd.MM.yyyy") ?? "Не указана";
        _ResultHours_Value_.Text = Math.Round(_HoursSlider_.Value).ToString("0");
        _ResultNotes_Value_.Text = string.IsNullOrWhiteSpace(_NotesEditor_.Text)
            ? "Не указана"
            : _NotesEditor_.Text.Trim();

        // Итог расположен сразу под кнопками. Не вызываем ScrollToAsync:
        // при появлении Border на Windows он иногда прокручивает к началу страницы.
        _ResultBorder_.IsVisible = true;
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        _isResetting = true;

        _NameEntry_.Text = string.Empty;
        _SecondEntry_.Text = string.Empty;
        _NotesEditor_.Text = string.Empty;
        _RolePicker_.SelectedIndex = -1;
        _OptionOneRadio_.IsChecked = true;
        _OptionTwoRadio_.IsChecked = false;
        _OptionThreeRadio_.IsChecked = false;
        _OptionFourRadio_.IsChecked = false;
        _OptionFiveRadio_.IsChecked = false;
        _ExtraOneCheck_.IsChecked = false;
        _ExtraTwoCheck_.IsChecked = false;
        _ExtraThreeCheck_.IsChecked = false;
        _ExtraFourCheck_.IsChecked = false;
        _ExtraFiveCheck_.IsChecked = false;
        _ExtraSixCheck_.IsChecked = false;
        _ExtraSevenCheck_.IsChecked = false;
        _FlexibleSwitch_.IsToggled = false;
        _StartDatePicker_.Date = new DateTime(2026, 11, 1);
        _HoursSlider_.Value = 10;

        _isResetting = false;
        UpdateRoleText();
        UpdateSubmitState();
        HideOldResult();
    }
}
