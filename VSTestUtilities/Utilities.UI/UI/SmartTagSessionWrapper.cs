﻿// Visual Studio Shared Project
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudioTools.TestUtilities.UI {
    public class SmartTagSessionWrapper : IIntellisenseSession {
#if DEV14_OR_LATER
        private readonly SessionHolder<ILightBulbSession> _sessionHolder;
        private readonly ILightBulbSession _session;

        public SmartTagSessionWrapper(SessionHolder<ILightBulbSession> sessionHolder) {
            _sessionHolder = sessionHolder;
            _session = _sessionHolder.Session;
        }
#else
        private readonly SessionHolder<ISmartTagSession> _sessionHolder;
        private readonly ISmartTagSession _session;

        public SmartTagSessionWrapper(SessionHolder<ISmartTagSession> sessionHolder) {
            _sessionHolder = sessionHolder;
            _session = _sessionHolder.Session;
        }
#endif

        public class SmartTagActionWrapper {
#if DEV14_OR_LATER
            private readonly ISuggestedAction _action;

            public SmartTagActionWrapper(ISuggestedAction action) {
                _action = action;
            }

#else
            private readonly ISmartTagAction _action;

            public SmartTagActionWrapper(ISmartTagAction action) {
                _action = action;
            }
#endif

            public string DisplayText {
                get { return _action.DisplayText; }
            }

            public void Invoke() {
#if DEV14_OR_LATER
                _action.Invoke(CancellationToken.None);
#else
                _action.Invoke();
#endif
            }
        }

        public IEnumerable<SmartTagActionWrapper> Actions {
            get {
#if DEV14_OR_LATER
                IEnumerable<SuggestedActionSet> sets;
                return _session.TryGetSuggestedActionSets(out sets) == QuerySuggestedActionCompletionStatus.Completed ?
                    sets.SelectMany(s => s.Actions).Select(a => new SmartTagActionWrapper(a)) :
                    Enumerable.Empty<SmartTagActionWrapper>();
#else
                return _session.ActionSets.SelectMany(s => s.Actions).Select(a => new SmartTagActionWrapper(a));
#endif

            }
        }

#region IIntellisenseSession Forwarders

        public bool IsDismissed {
            get {
                return _session.IsDismissed;
            }
        }

        public IIntellisensePresenter Presenter {
            get {
                return _session.Presenter;
            }
        }

        public PropertyCollection Properties {
            get {
                return _session.Properties;
            }
        }

        public ITextView TextView {
            get {
                return _session.TextView;
            }
        }

        public event EventHandler Dismissed {
            add { _session.Dismissed += value; }
            remove { _session.Dismissed -= value; }
        }

        public event EventHandler PresenterChanged {
            add { _session.PresenterChanged += value; }
            remove { _session.PresenterChanged -= value; }
        }

        public event EventHandler Recalculated {
            add { _session.Recalculated += value; }
            remove { _session.Recalculated -= value; }
        }

        public void Collapse() {
            _session.Collapse();
        }

        public void Dismiss() {
            _session.Dismiss();
        }

        public SnapshotPoint? GetTriggerPoint(ITextSnapshot textSnapshot) {
            return _session.GetTriggerPoint(textSnapshot);
        }

        public ITrackingPoint GetTriggerPoint(ITextBuffer textBuffer) {
            return _session.GetTriggerPoint(textBuffer);
        }

        public bool Match() {
            return _session.Match();
        }

        public void Recalculate() {
            _session.Recalculate();
        }

        public void Start() {
            _session.Start();
        }

#endregion
    }
}
