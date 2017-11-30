
# FaceApiWPF
Using the WebCam and send continuously pictures to the Azure FaceAPI. The analyzed results like age and emotion drawn on the WebCam stream.

# Configuration
Change the FaceApiKey to access the Azure FaceAPI
```xml
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1"/>
    </startup>
  <appSettings>
    <add key="FaceApiUrl" value="https://northeurope.api.cognitive.microsoft.com/face/v1.0" />
    <add key="FaceApiKey" value="YOUR KEY HERE" />
  </appSettings>
</configuration>
```

# Using
tbd

# Credits
Using Emgu CV (http://www.emgu.com) for accessing the WebCam.
