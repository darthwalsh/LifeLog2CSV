# LifeLog2CSV
[Sony LifeLog](https://www.sonymobile.com/us/apps-services/lifelog/) deprecated their web API, and [they don't provide a way to export sleep data](https://stackoverflow.com/questions/46079877/how-to-download-sony-lifelog-sleep-data).

Extract sleep data from by parsing screenshots of the mobile app.

# Getting Started
To use the app, for now you need to run it from Visual Studio. Eventually there will be a command line executable.

# Design
- LifeLogParser scrapes the image for sleep information.
- LifeLogGUI lets you debug the screen scraping by letting you watch where the scraping is looking. 

# Contribute
You can help by filing issues, fixing bugs, and sharing this with your friends.

If you want to send a pull request, please run test cases and format before submitting.

