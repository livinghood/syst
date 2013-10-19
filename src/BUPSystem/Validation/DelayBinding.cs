using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace BUPSystem
{
    public class DelayBinding
    {
        private readonly BindingExpressionBase _bindingExpression;
        private readonly DispatcherTimer _timer;

        protected DelayBinding(BindingExpressionBase bindingExpression, DependencyObject bindingTarget, DependencyProperty bindingTargetProperty, TimeSpan delay)
        {
            _bindingExpression = bindingExpression;

            // Subscribe to notifications for when the target property changes. This event handler will be 
            // invoked when the user types, clicks, or anything else which changes the target property
            var descriptor = DependencyPropertyDescriptor.FromProperty(bindingTargetProperty, bindingTarget.GetType());
            descriptor.AddValueChanged(bindingTarget, BindingTarget_TargetPropertyChanged);

            // Add support so that the Enter key causes an immediate commit
            var frameworkElement = bindingTarget as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.KeyUp += BindingTarget_KeyUp;
            }

            // Setup the timer, but it won't be started until changes are detected
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = delay;
        }

        private void BindingTarget_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            _timer.Stop();
            _bindingExpression.UpdateSource();
        }

        private void BindingTarget_TargetPropertyChanged(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            _bindingExpression.UpdateSource();
        }

        public static object SetBinding(DependencyObject bindingTarget, DependencyProperty bindingTargetProperty, TimeSpan delay, Binding binding)
        {
            // Override some specific settings to enable the behavior of delay binding
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            // Apply and evaluate the binding
            var bindingExpression = BindingOperations.SetBinding(bindingTarget, bindingTargetProperty, binding);

            // Setup the delay timer around the binding. This object will live as long as the target element lives, since it subscribes to the changing event, 
            // and will be garbage collected as soon as the element isn't required (e.g., when it's Window closes) and the timer has stopped.
            new DelayBinding(bindingExpression, bindingTarget, bindingTargetProperty, delay);

            // Return the current value of the binding (since it will have been evaluated because of the binding above)
            return bindingTarget.GetValue(bindingTargetProperty);
        }
    }
}
