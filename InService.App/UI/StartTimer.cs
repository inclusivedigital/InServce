using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InService.App.Data;
using InService.App.UI;
using InService.Lib;
using InService.Lib.Data;

namespace InService.App.UI
{
    public class StartTimer
    {
        private int _shortBreak = 300;
        private int _longBreak = 900;
        private int _pomodroTime = 0;
        private int _countseconds = 0;
        private int _longBreakInterval = 3;
        private int _currentPomodoro = 1;
        private int _totalBreak = 1;
        private TimerType _currentTimer = TimerType.POMODORO;
        private TimerState _timerState = TimerState.STOPPED;
        private Timer _timer = new Timer();
        private TextView TimerText;
        private Activity Activity;
        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }


        public StartTimer(Activity Activity, TextView timerText)
        {
            this.Activity = Activity; this.TimerText = timerText;
        }
        public void Start()
        {
            _timer.Interval = 1000;
            _timer.Elapsed += TimerElapsedEvent;

            TimerText.Text = secondsToCountdown();
            if (EndDate.HasValue)
            {
                _countseconds = (int)(EndDate.Value - StartDate).TotalSeconds;
                _timerState = TimerState.STOPPED;
                Activity.RunOnUiThread(SetTimerText);
            }
            else
            {
                _countseconds = (int)(DateTime.Now - StartDate).TotalSeconds;
                _timer.Start();
                _timerState = TimerState.RUNNING;
                Activity.RunOnUiThread(SetTimerText);
            }
        }

        private void TimerButtonClicked()
        {
            if (_timerState == TimerState.STOPPED)
            {
                _timer.Start();
                _timerState = TimerState.RUNNING;
            }
            else
            {
                _timer.Stop();
                _timerState = TimerState.STOPPED;
                //  GetNextTimer();
            }
            Activity.RunOnUiThread(SetTimerText);
        }
        private void TimerElapsedEvent(object sender, ElapsedEventArgs e)
        {
            _countseconds++;
            Activity.RunOnUiThread(DisplaySeconds);
            if (_countseconds == 0)
            {
                _timer.Stop();
                //   GetNextTimer();
                _timer.Start();
            }
        }

        private void GetNextTimer()
        {
            switch (_currentTimer)
            {
                case TimerType.POMODORO:
                    _currentPomodoro++;

                    if ((_totalBreak % _longBreakInterval) == 0)
                    {
                        _currentTimer = TimerType.LONGBREAK;
                        _countseconds = _longBreak;
                    }
                    else
                    {
                        _currentTimer = TimerType.SHORTBREAK;
                        _countseconds = _shortBreak;
                    }
                    break;
                case TimerType.SHORTBREAK:
                    _totalBreak++;
                    _currentTimer = TimerType.POMODORO;
                    _countseconds = _pomodroTime;

                    break;
                case TimerType.LONGBREAK:
                    _totalBreak++;
                    _currentTimer = TimerType.POMODORO;
                    _countseconds = _pomodroTime;
                    break;
            }
            Activity.RunOnUiThread(SetTimerBackground);
        }
        private string secondsToCountdown()
        {
            int mins = _countseconds / 60;
            int seconds = _countseconds - mins * 60;
            if (seconds >= 5)
            {
                TimerText.SetTextColor(Android.Graphics.Color.DarkGreen);
                // TextDescription.Text = "Looking for you...";
            }
            if (seconds >= 10)
            {
                TimerText.SetTextColor(Android.Graphics.Color.Green);
                // TextDescription.Text = "Move your hand a little closer...";
            }
            if (seconds >= 30)
            {
                TimerText.SetTextColor(Android.Graphics.Color.BlueViolet);
                // TextDescription.Text = "Even closer...";
            }
            if (seconds >= 40)
            {
                TimerText.SetTextColor(Android.Graphics.Color.Red);
                // TextDescription.Text = "You can start again...";
            }
            return string.Format("{0}:{1}", mins.ToString("00"), seconds.ToString("00"));
        }

        private void DisplaySeconds()
        {

            TimerText.Text = secondsToCountdown();
        }
        private void SetTimerBackground()
        {


            if (_currentTimer == TimerType.POMODORO)
            {
                TimerText.SetBackgroundColor(Android.Graphics.Color.Red);
            }
            else
            {
                TimerText.SetBackgroundColor(Android.Graphics.Color.Blue);
            }
        }

        private void SetTimerText()
        {
            DisplaySeconds();
        }

    }
}