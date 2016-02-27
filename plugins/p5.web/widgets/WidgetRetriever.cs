/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating retrieving web widgets
    /// </summary>
    public class WidgetRetriever : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetRetriever"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetRetriever (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        #region [ -- Active Events for retrieving widgets -- ]

        /// <summary>
        ///     Returns the ID and type of the given widget's parent(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-parent-widget", Protection = EventProtection.LambdaClosed)]
        public void get_parent_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given
                foreach (var idxWidget in FindWidgets <Control> (context, e.Args, "get-parent-widget")) {

                    // Returning parent of widget, and parent's typename
                    e.Args.Add(GetTypeName (idxWidget.Parent), idxWidget.Parent.ID);
                }
            }
        }

        /// <summary>
        ///     Returns the ID and type of the given widget's children
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-children-widgets", Protection = EventProtection.LambdaClosed)]
        public void get_children_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given
                foreach (var idxWidget in FindWidgets <Control> (context, e.Args, "get-children-widgets")) {

                    // Adding currently iterated widget's ID
                    e.Args.Add(idxWidget.ID);

                    // Then looping through currently iterated widget's children, adding them
                    foreach (Control idxCtrl in idxWidget.Controls) {

                        // Adding type of widget as name, and ID as value
                        e.Args.LastChild.Add(GetTypeName (idxCtrl), idxCtrl.ID);
                    }
                }
            }
        }

        /// <summary>
        ///     Find widget(s) according to criteria underneath an (optionally) declared widget, and returns its ID
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "find-widget", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "find-widget-like", Protection = EventProtection.LambdaClosed)]
        public void find_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving widget from where to start search, defaulting to "cnt"
                var parentControl = FindControl<Widget> (e.Args.GetExValue (context, "cnt"), Manager.AjaxPage);

                // Retrieving all controls having properties matching whatever arguments supplied
                foreach (var idxWidget in FindWidgetsBy (e.Args, parentControl, context, e.Name == "find-widget-like")) {

                    // Adding type of widget as name, and ID as value
                    e.Args.Add (GetTypeName (idxWidget), idxWidget.ID);
                }
            }
        }

        /// <summary>
        ///     Finds first ancestor widget matching given to criteria
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "find-first-ancestor-widget", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "find-first-ancestor-widget-like", Protection = EventProtection.LambdaClosed)]
        public void find_first_ancestor_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Sanity check
                if (e.Args.Children.Count (ix => ix.Name != "") == 0)
                    throw new LambdaException ("No criteria submitted to [find-ancestor-widget]", e.Args, context);

                // Retrieving widget from where to start search, defaulting to "cnt"
                var startCtrl = FindControl<Control> (XUtil.Single<string> (context, e.Args, true), Manager.AjaxPage);

                // Sanity check
                if (startCtrl == null)
                    throw new LambdaException ("Start control not found for [find-ancestor-widget]", e.Args, context);

                bool like = e.Name == "find-first-ancestor-widget-like";

                // Looping upwards in ancestor hierarchy, til either startCtrl is null, or given attribute(s) are found on a widget,
                // with the (alternatively) supplied value
                startCtrl = startCtrl.Parent;
                while (startCtrl != null) {
                    Widget curIdxWidget = startCtrl as Widget;
                    if (curIdxWidget != null) {
                        bool found = true;
                        foreach (var idxChildNode in e.Args.Children) {
                            if (!curIdxWidget.HasAttribute (idxChildNode.Name)) {
                                found = false;
                                break;
                            }
                            if (idxChildNode.Value == null) {

                                // Do nothing, this is a match
                            } else {
                                if (curIdxWidget [idxChildNode.Name] != null) {
                                    if (like) {
                                        if (!curIdxWidget [idxChildNode.Name].Contains (idxChildNode.GetExValue<string> (context, null))) {
                                            found = false;
                                            break;
                                        }
                                    } else {
                                        if (curIdxWidget [idxChildNode.Name] != idxChildNode.GetExValue<string> (context, null)) {
                                            found = false;
                                            break;
                                        }
                                    }
                                } else {
                                    found = false;
                                    break;
                                }
                            }
                        }
                        if (found) {

                            // We found our first matching ancestor widget!
                            e.Args.Value = curIdxWidget.ID;
                            return;
                        }
                    }
                    startCtrl = startCtrl.Parent;
                }
            }

            // We didn't find any widgets matching criteria
            e.Args.Value = null;
        }

        /// <summary>
        ///     Lists all widgets on page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widgets", Protection = EventProtection.LambdaClosed)]
        public void list_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving filter, if any
                var filter = XUtil.Iterate<string>(context, e.Args).ToList ();
                if (e.Args.Value != null && filter.Count == 0)
                    return; // Possibly a filter expression, leading into oblivion

                // Recursively retrieving all widgets on page, matching filter, or all, if there is no filters
                ListWidgets(filter, e.Args, Manager.AjaxPage);
            }
        }

        /// <summary>
        ///     Checks if the given widget(s) exist
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "widget-exist", Protection = EventProtection.LambdaClosed)]
        public void widget_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args, true)) {

                // Looping through all IDs given
                foreach (var widgetId in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Adding a boolean as result node, with widget's ID as name, indicating existence of widget
                    e.Args.Add(widgetId, FindControl<Widget>(widgetId, Manager.AjaxPage) != null);
                }
            }
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Recursively traverses entire Control hierarchy on page, and adds up into result node
         */
        private static void ListWidgets (
            List<string> filter, 
            Node args, 
            Control ctrl)
        {
            // Checking if current ctrl is mathing filters supplied, 
            // alternative there are no filters, at which point everything should match
            if (filter.Count == 0 || filter.Count(ix => ix == ctrl.ID) > 0) {

                // Current Control is matching one of our filters, or there are no filters
                args.Add(GetTypeName (ctrl), ctrl.ID);
            }

            // Looping through each child control of current control, recursively invoking "self" to check for match
            foreach (Control idxChildCtrl in ctrl.Controls) {

                // Recursively invoking "self"
                ListWidgets (filter, args, idxChildCtrl);
            }
        }

        /*
         * Helper to retrieve a list of widgets from a Node that serves as criteria
         */
        private IEnumerable<Widget> FindWidgetsBy (
            Node args, 
            Control ctrl, 
            ApplicationContext context,
            bool like = false)
        {
            // Checking if current ctrl Widget, at which point we can check for match against criteria
            var widget = ctrl as Widget;
            if (widget != null) {

                // Looping through each criteria, to check for match
                // Notice, ALL specified criteria must match for control to yield a match
                // If "like" is true, then it will look for a "like" condition, meaning not exact match of criteria, but rather 
                // if attribute "contains" value of criteria
                bool match = true;
                foreach (var idxNode in args.Children) {
                    if (!widget.HasAttribute (idxNode.Name)) {
                        match = false;
                        break;
                    } else {
                        var value = idxNode.GetExValue<string> (context, null);
                        if (value == null && widget.HasAttribute (idxNode.Name)) {

                            // Match, since caller just looked for existence of attribute
                        } else if (!like && widget [idxNode.Name] != value) {
                            match = false;
                            break;
                        } else if (like && !widget [idxNode.Name].Contains (value)) {
                            match = false;
                            break;
                        }
                    }
                }

                // Checking if criteria search above gave us a match
                if (match)
                    yield return widget; // We have a match!
            }

            // Regardless of whether or not current control gave us a match, we still search children collection for matches
            foreach (Control idxChildCtrl in ctrl.Controls) {

                // Recursively invoking "self" with currently iterated child control
                // Notice, the "ToList" parts are important here, to not break enumeration upon finding a match
                foreach (var idxSubFind in FindWidgetsBy (args, idxChildCtrl, context, like).ToList ()) {
                    yield return idxSubFind; // We have a match!
                }
            }
        }

        #endregion
    }
}
