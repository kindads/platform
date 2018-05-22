// Declare variables
var IdUser = IdUserIdentity;
var connection = $.hubConnection();
var mailFlowHubProxy = connection.createHubProxy('notificationHub');

// Attach client function
mailFlowHubProxy.on('showNotification', function (title, message) { 

    // show the notification tile
    showNotification(title, message);
});

function showNotification(title,message) {
    //Checar hasta que se libere  
    if( currentMessage === 1) {
      setTimeout(function () {    
            showNotification(title, message);
        }, 8000);        
    }
    else {
        currentMessage = 1;

        console.log("Titulo:", title);
        console.log("Message:", message);

        // Set content in tile
        $("#notification-title").text(title);
        $("#notification-message").text(message);

        // Show tile
      $("#notification-tile").show("fast");
      $('#soundNotification')[0].play();
    } 
}

// Attach client function
mailFlowHubProxy.on('showError', function (message) {
    //Todo: Show tile and start push notification animation
    alert(message);
});

// Start communication with Server called StartHandler in SignalRHub.cs
connection.start()
    .done(function () {    
        console.log("Connected");
      mailFlowHubProxy.invoke('StartHandlerRx', IdUser);
    })
    .fail(function () { console.log('Could not connect'); });

// Wait method
function wait(ms) {
    var start = new Date().getTime();
    var end = start;
    while (end < start + ms) {
        end = new Date().getTime();
    }
}
