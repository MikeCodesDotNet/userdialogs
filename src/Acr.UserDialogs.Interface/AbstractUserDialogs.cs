﻿using System;
using System.Threading.Tasks;


namespace Acr.UserDialogs {

    public abstract class AbstractUserDialogs : IUserDialogs {

        public abstract void Alert(AlertConfig config);
        public abstract void ActionSheet(ActionSheetConfig config);
        public abstract void Confirm(ConfirmConfig config);
        public abstract void Login(LoginConfig config);
        public abstract void Prompt(PromptConfig config);
        public abstract void Toast(string message, int timeoutSeconds, Action onClick, MaskType maskType);
        public abstract void ShowError(string message, int timeoutSeconds);
        public abstract void ShowSuccess(string message, int timeoutSeconds);
        protected abstract IProgressDialog CreateDialogInstance();


        public virtual Task<string> ActionSheetAsync(string title, string cancel, string destructive, params string[] buttons) {
            var tcs = new TaskCompletionSource<string>();
            var cfg = new ActionSheetConfig();
            if (title != null)
                cfg.Title = title;

            if (cancel != null)
                cfg.SetCancel(cancel, () => tcs.TrySetResult(cancel));

            if (destructive != null)
                cfg.SetDestructive(destructive, () => tcs.TrySetResult(destructive));

            foreach (var btn in buttons)
                cfg.Add(btn, () => tcs.TrySetResult(btn));

			this.ActionSheet(cfg);
            return tcs.Task;
        }


        public virtual void Alert(string message, string title, string okText) {
            this.Alert(new AlertConfig {
                Message = message,
                Title = title,
                OkText = okText ?? AlertConfig.DefaultOkText
            });
        }


        private IProgressDialog loading;
		public virtual void ShowLoading(string title, MaskType? maskType) {
            if (this.loading == null)
				this.loading = this.Loading(title, null, null, true, maskType);
        }


        public virtual void HideLoading() {
            if (this.loading != null) {
                this.loading.Dispose();
                this.loading = null;
            }
        }


		public virtual IProgressDialog Loading(string title, Action onCancel, string cancelText, bool show, MaskType? maskType) {
            return this.Progress(new ProgressDialogConfig {
                Title = title ?? ProgressDialogConfig.DefaultTitle,
                AutoShow = show,
                CancelText = cancelText ?? ProgressDialogConfig.DefaultCancelText,
				MaskType = maskType ?? ProgressDialogConfig.DefaultMaskType,
                IsDeterministic = false,
                OnCancel = onCancel
            });
        }


		public virtual IProgressDialog Progress(string title, Action onCancel, string cancelText, bool show, MaskType? maskType) {
			return this.Progress(new ProgressDialogConfig {
                Title = title ?? ProgressDialogConfig.DefaultTitle,
                AutoShow = show,
                CancelText = cancelText ?? ProgressDialogConfig.DefaultCancelText,
				MaskType = maskType ?? ProgressDialogConfig.DefaultMaskType,
                IsDeterministic = true,
                OnCancel = onCancel
            });
        }


		public virtual IProgressDialog Progress(ProgressDialogConfig config) {
            var dlg = this.CreateDialogInstance();
            dlg.Title = config.Title;
            dlg.IsDeterministic = config.IsDeterministic;
			dlg.MaskType = config.MaskType;

            if (config.OnCancel != null)
                dlg.SetCancel(config.OnCancel, config.CancelText);

            if (config.AutoShow)
                dlg.Show();

            return dlg;
        }


        public virtual Task AlertAsync(string message, string title, string okText) {
            var tcs = new TaskCompletionSource<object>();
            this.Alert(new AlertConfig {
                Message = message,
                Title = title,
                OkText = okText ?? AlertConfig.DefaultOkText,
                OnOk = () => tcs.TrySetResult(null)
            });
            return tcs.Task;
        }


        public virtual Task AlertAsync(AlertConfig config) {
            var tcs = new TaskCompletionSource<object>();
            config.OnOk = () => tcs.TrySetResult(null);
            this.Alert(config);
            return tcs.Task;
        }


        public virtual Task<bool> ConfirmAsync(string message, string title, string okText, string cancelText) {
            var tcs = new TaskCompletionSource<bool>();
            this.Confirm(new ConfirmConfig {
                Message = message,
                Title = title,
                CancelText = cancelText ?? ConfirmConfig.DefaultCancelText,
                OkText = okText ?? ConfirmConfig.DefaultOkText,
                OnConfirm = x => tcs.TrySetResult(x)
            });
            return tcs.Task;
        }


        public virtual Task<bool> ConfirmAsync(ConfirmConfig config) {
            var tcs = new TaskCompletionSource<bool>();
            config.OnConfirm = x => tcs.TrySetResult(x);
            this.Confirm(config);
            return tcs.Task;
        }


        public virtual Task<LoginResult> LoginAsync(string title, string message) {
            return this.LoginAsync(new LoginConfig {
                Title = title ?? LoginConfig.DefaultTitle,
                Message = message
            });
        }


        public virtual Task<LoginResult> LoginAsync(LoginConfig config) {
            var tcs = new TaskCompletionSource<LoginResult>();
            config.OnResult = x => tcs.TrySetResult(x);
            this.Login(config);
            return tcs.Task;
        }


        public virtual Task<PromptResult> PromptAsync(string message, string title, string okText, string cancelText, string placeholder, InputType inputType) {
            var tcs = new TaskCompletionSource<PromptResult>();
            this.Prompt(new PromptConfig {
                Message = message,
                Title = title,
                CancelText = cancelText ?? PromptConfig.DefaultCancelText,
                OkText = okText ?? PromptConfig.DefaultOkText,
                Placeholder = placeholder,
                InputType = inputType,
                OnResult = x => tcs.TrySetResult(x)
            });
            return tcs.Task;
        }


        public virtual Task<PromptResult> PromptAsync(PromptConfig config) {
            var tcs = new TaskCompletionSource<PromptResult>();
            config.OnResult = x => tcs.TrySetResult(x);
            this.Prompt(config);
            return tcs.Task;
        }
    }
}
