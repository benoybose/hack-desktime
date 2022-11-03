#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DeskTime
{
    internal class Browsers
    {
        public enum Browser
        {
            IExplore,
            MicrosoftEdge,
            Firefox,
            Opera,
            Chrome
        }

        private DdeClient browserFF;

        private DdeClient browserIE;

        private DdeClient browserOP;

        private DdeClient browserCh;

        private volatile Dictionary<int, AutomationElement> chromeAutomationElementsList = new Dictionary<int, AutomationElement>();

        private volatile Dictionary<int, AutomationElement> firefoxAutomationElementsList = new Dictionary<int, AutomationElement>();

        internal string GetBrowserUrl(IntPtr hWnd, Browser browser, bool trytwo = false)
        {
            string item = "URL";
            try
            {
                DdeClient ddeClient;
                switch (browser)
                {
                    case Browser.Firefox:
                        ddeClient = ((browserFF != null) ? browserFF : (browserFF = new DdeClient("Firefox", "WWW_GetWindowInfo")));
                        break;
                    case Browser.IExplore:
                        ddeClient = ((browserIE != null) ? browserIE : (browserIE = new DdeClient("IExplore", "WWW_GetWindowInfo")));
                        item = "0xFFFFFFFF";
                        break;
                    case Browser.Opera:
                        ddeClient = ((browserOP != null) ? browserOP : (browserOP = new DdeClient("Opera", "WWW_GetWindowInfo")));
                        break;
                    case Browser.Chrome:
                        ddeClient = ((browserCh != null) ? browserCh : (browserCh = new DdeClient("Google Chrome", "WWW_GetWindowInfo")));
                        break;
                    default:
                        return null;
                }
                string[] array = ddeClient.GetData(item, 1000).Split(',');
                array[0] = Regex.Replace(array[0], "^\"|\"$", "");
                return array[0];
            }
            catch (Exception)
            {
                switch (browser)
                {
                    case Browser.Firefox:
                        if (browserFF != null)
                        {
                            browserFF.Dispose();
                            browserFF = null;
                        }
                        break;
                    case Browser.IExplore:
                        if (browserIE != null)
                        {
                            browserIE.Dispose();
                            browserIE = null;
                        }
                        break;
                    case Browser.Opera:
                        if (browserOP != null)
                        {
                            browserOP.Dispose();
                            browserOP = null;
                        }
                        break;
                    case Browser.Chrome:
                        if (browserCh != null)
                        {
                            browserCh.Dispose();
                            browserCh = null;
                        }
                        break;
                }
                if (!trytwo)
                {
                    return GetBrowserUrl(hWnd, browser, trytwo: true);
                }
                return null;
            }
        }

        private AutomationElement GetAutomationElement(Process process)
        {
            if (process == null)
            {
                return null;
            }
            if (process.MainWindowHandle == IntPtr.Zero)
            {
                return null;
            }
            return AutomationElement.FromHandle(process.MainWindowHandle);
        }

        internal string GetFirefoxAutomationUrl(IntPtr hWnd)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                AutomationElement automationElement = null;
                AutomationElement element = AutomationElement.FromHandle(hWnd);
                if (element == null)
                {
                    return null;
                }
                int key = element.GetRuntimeId()[1];
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Runtime ID: " + key);
                if (firefoxAutomationElementsList.ContainsKey(key))
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ">--------->>> Cached firefoxSearchBox element! Runtime ID: " + key);
                    automationElement = firefoxAutomationElementsList[key];
                }
                else
                {
                    Task<AutomationElement> task = Task.Run(delegate
                    {
                        Condition condition = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "search or enter address", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "search with google or enter address", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "ieraksti meklējamo tekstu vai mājas lapas adresi", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "введите поисковый запрос или адрес", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Mit Google suchen oder Adresse eingeben", PropertyConditionFlags.IgnoreCase));
                        return element.FindFirst(TreeScope.Subtree, new AndCondition(new OrCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text)), condition));
                    });
                    automationElement = ((!task.Wait(TimeSpan.FromSeconds(15.0))) ? null : task.Result);
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "firefoxAutomationElements: " + firefoxAutomationElementsList.Count);
                    if (firefoxAutomationElementsList.Count > 30)
                    {
                        firefoxAutomationElementsList.Clear();
                    }
                    if (automationElement != null)
                    {
                        firefoxAutomationElementsList.Add(key, automationElement);
                    }
                }
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Time taken: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
                if (automationElement != null)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get Firefox SearchBox url!");
                    return ((ValuePattern)automationElement.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                }
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Firefox SearchBox not found");
                if (firefoxAutomationElementsList.ContainsKey(key))
                {
                    firefoxAutomationElementsList.Remove(key);
                }
                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "firefox url not found");
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                return null;
            }
        }

        internal string GetFirefoxAutomationUrl_X(IntPtr hWnd)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                AutomationElement automationElement = null;
                AutomationElement automationElement2 = AutomationElement.FromHandle(hWnd);
                if (automationElement2 == null)
                {
                    return null;
                }
                int num = automationElement2.GetRuntimeId()[1];
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Runtime ID: " + num);
                Condition condition = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "Navigation", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Navigation Toolbar", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Navigācijas rīkjosla", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Панель навигации", PropertyConditionFlags.IgnoreCase));
                condition = new AndCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar), condition);
                if (automationElement2.FindFirst(TreeScope.Children, condition) != null)
                {
                    condition = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "search or enter address", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "search with google or enter address", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "ieraksti meklējamo tekstu vai mājas lapas adresi", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "введите поисковый запрос или адрес", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Mit Google suchen oder Adresse eingeben", PropertyConditionFlags.IgnoreCase));
                    automationElement = automationElement2.FindFirst(TreeScope.Subtree, new AndCondition(new OrCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text)), condition));
                }
                else
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Firefox Toolbar not found");
                }
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "firefoxAutomationElements: " + firefoxAutomationElementsList.Count);
                if (firefoxAutomationElementsList.Count > 30)
                {
                    firefoxAutomationElementsList.Clear();
                }
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Time taken: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
                if (automationElement != null)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Get Firefox SearchBox url!");
                    return ((ValuePattern)automationElement.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                }
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Firefox SearchBox not found");
                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "firefox url not found");
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                return null;
            }
        }

        internal string GetChromeUrlFromAutomatioElement(AutomationElement element)
        {
            string result = null;
            Condition condition = new OrCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document), new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_RenderWidgetHostHWND"));
            AutomationElement automationElement = element.FindFirst(TreeScope.Descendants, condition);
            if (automationElement != null)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "get pattern form ChromeDocument");
                try
                {
                    result = ((ValuePattern)automationElement.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                    return result;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ex.Message);
                    return result;
                }
            }
            return result;
        }

        internal string GetOperaUrlFromAutomatioElement(AutomationElement element)
        {
            string text = null;
            Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Document);
            AutomationElement automationElement = element.FindFirst(TreeScope.Descendants, condition);
            if (automationElement != null)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "get pattern form OperaDocument");
                try
                {
                    text = ((ValuePattern)automationElement.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ex.Message);
                }
            }
            if (text == null)
            {
                Condition condition2 = new PropertyCondition(AutomationElement.NameProperty, "Address field");
                AutomationElement automationElement2 = element.FindFirst(TreeScope.Descendants, condition2);
                if (automationElement2 != null)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "get pattern form AddressField");
                    try
                    {
                        text = ((ValuePattern)automationElement2.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                        return text;
                    }
                    catch (InvalidOperationException ex2)
                    {
                        Console.WriteLine(ex2.Message);
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ex2.Message);
                        return text;
                    }
                }
            }
            return text;
        }

        internal string GetChromeAutomationUrl(IntPtr hWnd)
        {
            try
            {
                string result = null;
                Stopwatch stopwatch = Stopwatch.StartNew();
                AutomationElement automationElement = null;
                AutomationElement element = AutomationElement.FromHandle(hWnd);
                if (element == null)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Chrome AutomationElement not found");
                    return null;
                }
                int key = element.GetRuntimeId()[1];
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Runtime ID: " + key);
                if (chromeAutomationElementsList.ContainsKey(key))
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ">--------->>> Cached chromeSearchBox element! Runtime ID: " + key);
                    automationElement = chromeAutomationElementsList[key];
                }
                else
                {
                    Task<AutomationElement> task = Task.Run(() => element.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AccessKeyProperty, "Ctrl+L", PropertyConditionFlags.IgnoreCase)));
                    if (task.Wait(TimeSpan.FromSeconds(15.0)))
                    {
                        automationElement = task.Result;
                        if (automationElement == null)
                        {
                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "chromeSearchBox by Ctrl+L not found");
                            Condition condition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, "main", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                            AutomationElement automationElement2 = element.FindFirst(TreeScope.Descendants, condition);
                            if (automationElement2 != null)
                            {
                                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "found google main toolbar");
                                Condition condition2 = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "Address and search bar", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Adreses un meklēšanas josla", PropertyConditionFlags.IgnoreCase), new PropertyCondition(AutomationElement.NameProperty, "Адресная строка и строка поиска", PropertyConditionFlags.IgnoreCase));
                                Condition condition3 = new OrCondition(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                                Condition condition4 = new AndCondition(condition3, condition2);
                                automationElement = automationElement2.FindFirst(TreeScope.Descendants, condition4);
                            }
                        }
                        else
                        {
                            Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "FOUND chromeSearchBox by Ctrl+L");
                        }
                    }
                    else
                    {
                        automationElement = null;
                    }
                    if (chromeAutomationElementsList.Count > 500)
                    {
                        chromeAutomationElementsList.Clear();
                    }
                    if (automationElement != null)
                    {
                        chromeAutomationElementsList.Add(key, automationElement);
                    }
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "chromeAutomationElementsList: " + chromeAutomationElementsList.Count);
                }
                stopwatch.Stop();
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Time taken: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
                if (automationElement != null)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "get pattern form Chrome SearchBox");
                    try
                    {
                        result = ((ValuePattern)automationElement.GetCurrentPattern(ValuePattern.Pattern)).Current.Value;
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + ex.Message);
                        if (chromeAutomationElementsList.ContainsKey(key))
                        {
                            chromeAutomationElementsList.Remove(key);
                        }
                    }
                }
                else
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Chrome SearchBox NOT FOUND");
                    if (chromeAutomationElementsList.ContainsKey(key))
                    {
                        chromeAutomationElementsList.Remove(key);
                    }
                }
                return result;
            }
            catch (Exception ex2)
            {
                chromeAutomationElementsList.Clear();
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "chorme url not found");
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex2.ToString());
                return null;
            }
        }

        internal string GetOperaAutomationUrl(IntPtr hWnd)
        {
            try
            {
                string text = null;
                Stopwatch.StartNew();
                AutomationElement automationElement = AutomationElement.FromHandle(hWnd);
                if (automationElement == null)
                {
                    Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Opera AutomationElement not found");
                    return null;
                }
                text = GetOperaUrlFromAutomatioElement(automationElement);
                if (text != null)
                {
                    return text;
                }
                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "opera url not found");
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                return null;
            }
        }

        internal string GetEdgeAutomationUrl(Process process)
        {
            AutomationElement automationElement = GetAutomationElement(process);
            if (automationElement == null)
            {
                return null;
            }
            try
            {
                AutomationElement automationElement2 = automationElement.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, "addressEditBox"));
                return (automationElement2 != null) ? ((ValuePattern)automationElement2.GetCurrentPattern(ValuePattern.Pattern)).Current.Value : null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "edge url not found");
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                return null;
            }
        }

        internal string GetMsEdgeAutomationUrl(IntPtr hWnd)
        {
            AutomationElement automationElement = AutomationElement.FromHandle(hWnd);
            if (automationElement == null)
            {
                return null;
            }
            try
            {
                AutomationElement automationElement2 = automationElement.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "OmniboxViewViews"));
                return (automationElement2 != null) ? ((ValuePattern)automationElement2.GetCurrentPattern(ValuePattern.Pattern)).Current.Value : null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "MS Edge url not found");
                Trace.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + "Exception: " + ex.ToString());
                return null;
            }
        }
    }
}
