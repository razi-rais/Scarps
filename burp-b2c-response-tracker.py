from burp import IBurpExtender
from burp import IHttpListener
from burp import IProxyListener
from burp import IScannerListener
from burp import IExtensionStateListener
from java.io import PrintWriter

class BurpExtender(IBurpExtender, IHttpListener, IProxyListener, IScannerListener, IExtensionStateListener):
    
    #
    # implement IBurpExtender
    #
    
    def	registerExtenderCallbacks(self, callbacks):
        # keep a reference to our callbacks object
        self._callbacks = callbacks
        self.helpers =  self._callbacks.getHelpers()

        
        # set our extension name
        callbacks.setExtensionName("AzureADB2C-Response-Headers-Cookies")
        
        # obtain our output stream
        self._stdout = PrintWriter(callbacks.getStdout(), True)

        # register ourselves as an HTTP listener
        callbacks.registerHttpListener(self)
        
        # register ourselves as a Proxy listener
        callbacks.registerProxyListener(self)
        
        # register ourselves as a Scanner listener
        callbacks.registerScannerListener(self)
        
        # register ourselves as an extension state listener
        callbacks.registerExtensionStateListener(self)
    
    #
    # implement IHttpListener
    #

    def processHttpMessage(self, toolFlag, messageIsRequest, messageInfo):
        self._stdout.println(
                ("HTTP request to " if messageIsRequest else "HTTP response from ") +
                messageInfo.getHttpService().toString() +
                " [" + self._callbacks.getToolName(toolFlag) + "]")
        if not messageIsRequest:
            response_bytes = messageInfo.getResponse()
            resp_as_string = self.helpers.bytesToString(response_bytes)
            self._stdout.println("HTTP response from:" + messageInfo.getHttpService().toString())
            resp_info = self.helpers.analyzeResponse(response_bytes)

            content = ""
            try:
                correlationid = resp_as_string.split("<!-- CorrelationId:")[1].split("-->")[0].strip()
                content = "\n----------------------------------------"
                content += "\nHTTP response from: " + messageInfo.getHttpService().toString()
                content += "\nCorrelationId: "+ correlationid 
                content += "\n----------------------------------------\n"
                #content += "\nHeaders & Cookies \n" 
                # content += "\n"+ resp_info.headers.toString()
                for header in resp_info.headers:
                    self._stdout.println(header)
                    content += header+"\n\n"
                #content += "\n*********** Cookies *******************\n" 
                # for cookie in resp_info.getCookies():
                #     self._stdout.println(cookie.getName()+": "+cookie.getValue())
                #     content += cookie.getName()+": "+cookie.getValue()+"\n"

                with open(correlationid+".txt", "a") as text_file:
                    text_file.write(content)
            except:
                self._stdout.println("correlationId not found. Skipping HTTP response from:" + messageInfo.getHttpService().toString())
                pass

    #
    # implement IProxyListener
    #

    def processProxyMessage(self, messageIsRequest, message):
        self._stdout.println(
                ("Proxy request to " if messageIsRequest else "Proxy response from ") +
                message.getMessageInfo().getHttpService().toString())

    #
    # implement IScannerListener
    #

    def newScanIssue(self, issue):
        self._stdout.println("New scan issue: " + issue.getIssueName())

    #
    # implement IExtensionStateListener
    #

    def extensionUnloaded(self):
        self._stdout.println("Extension was unloaded")
