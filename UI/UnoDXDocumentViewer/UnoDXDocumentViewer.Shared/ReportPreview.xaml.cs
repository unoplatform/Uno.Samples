using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace kahua.host.uno.ui.reporting
{
    public sealed partial class ReportPreview : UserControl
    {
        public ReportPreview()
        {
            this.InitializeComponent();

            Console.WriteLine($"{nameof(ReportPreview)}.ctor");

            Background = new SolidColorBrush(Colors.Transparent);
            Loaded += ReportPreview_Loaded;
        }

        private void ReportPreview_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"{nameof(ReportPreview)}.ReportPreview_Loaded");

            TrySetReport();
        }

        private void TrySetReport()
        {

#if __WASM__
            var htmlId = $"Report_{this.GetHtmlId()}";
            this.htmlReport.HtmlContent = $@"<div id=""{htmlId}"" style=""height:100%;width:100%;"" data-bind=""dxReportViewer: viewerOptions""></div>";

            var javascript = @"
                require(
                [`${config.uno_app_base}/Assets/js/jquery.min.js`, `${config.uno_app_base}/Assets/js/jquery-ui.min.js`, `${config.uno_app_base}/Assets/js/knockout-latest.js`],
                ($, jqueryUi, ko) => {
                    //debugger;
                    window.ko = ko;

                    console.log('window.$: ' + window.$);
                    console.log('window.jQuery: ' + window.jQuery);
                    console.log('window.$.ui: ' + window.$.ui);
                    console.log('window.jQuery.ui: ' + window.jQuery.ui);
                    console.log('window.ko: ' + window.ko);

                    require(
                        [`${config.uno_app_base}/Assets/js/dx.all.js`],
                        (DevExpress) => {
                            console.log('DevExpress: ' + DevExpress);
                            console.log('window.DevExpress: ' + window.DevExpress);

                            require(
                                [`${config.uno_app_base}/Assets/js/dx-analytics-core.js`],
                                (DXAnalytics) => {
                                    console.log('DXAnalytics: ' + DXAnalytics);
                        
                                    require(
                                        [`${config.uno_app_base}/Assets/js/dx-webdocumentviewer.js`],
                                        (WebDocumentViewer) => {
                                            console.log('WebDocumentViewer: ' + WebDocumentViewer);
                                           // debugger;

                                            console.log('DocumentViewer init');

                                            const invokeAction = 'DXXRDV',
                                                host = 'http://localhost:15099/', // URI of your backend project.
                                                reportUrl = 'TestReport';

                                            window.documentPreview = null;
                                            var documentPreviewViewModel = {
                                                viewerOptions: {
                                                    reportUrl: reportUrl, // The URL of a report that is opened in the Document Viewer when the application starts.
                                                    requestOptions: { // Options for processing requests from the Web Document Viewer.  
                                                        host: host,
                                                        invokeAction: invokeAction // The URI path of the controller action that processes requests.
                                                    }
                                                }
                                            };

                                           // debugger;
                                            window.ko.applyBindings(documentPreviewViewModel);

                                            console.log('DocumentViewer init DONE');
                                        }
                                    );
                                }
                            );
                        }
                    );
                }
            );";
            this.htmlReport.JsContent = javascript;

#endif

            Console.WriteLine($"{nameof(ReportPreview)}.TrySetReport complete");
        }
    }
}
