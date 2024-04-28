"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Toggle Chat
$('#chat_btn').on('click', function () {
    $('#chat_box').toggle();
});

// Disable the send button until connection is established.
$("#sendButton").prop("disabled", true);

// Start the connection
connection.start().then(function () {
    $("#sendButton").prop("disabled", false);
}).catch(function (err) {
    console.error(err.toString());
});

// Method to receive and display messages
connection.on("ReceiveMessageUser", function (message) {
    console.log("Received message: " + message);
    $("#messagesList").append(`<p class="bg-white p-3 w-fit rounded-xl">${message}</p>`);
});


$("#sendButton").on('click', function (event) {
    // Get admin Id (Destination user)
    var message = $("#messageInput").val();

    $("#messagesList").append(`<div class="flex gap-2"><p class="bg-MyOrange text-white p-3 w-fit rounded-xl ml-auto">${message}</p></div>`);
    connection.invoke("SendMessageToUser", message).then(function () {
        // Empty message box
        $("#messageInput").val('');
    }).catch(function (err) {
        console.error(err.toString());
    });
    event.preventDefault();
});