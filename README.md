
# FaceApiWPF
Using the WebCam and send continuously pictures to the Azure FaceAPI. The analyzed results like age and emotions (anger, contemp, disgust, fear, happiness, neutral, sadness, surprise) drawn on the WebCam stream.

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
![Screenshot](/FaceApiWPFScreenshot.png)
1. Start/Stop the WebCam capture and sends the each 500ms (can be configured under 5) a picture for analyzing to Azure FaceAPI
2. Take a picture of the WebCam and sends it for analyzing to Azure FaceAPI
3. Zoom in or out of the WebCam
4. Setting the resolution of the WebCam (possible values: 320x240, 640x480, 960x720, 1280x960)
5. Configure the time, when an image should be send to Azure FaceAPI for analyzing (possible values every: 150ms, 300ms, 500ms, 1000ms, disabled)
6. Show the current frames per seconds, zoom-value (button 3), resolution (button 4) and timeframe for sending the image to Azure FaceAPI (button 5)

# Credits
Using Emgu CV (http://www.emgu.com) for accessing the WebCam.
