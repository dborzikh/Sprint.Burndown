"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notification").build();

connection.on('started', function (totalItems) {
    console.info('loading -> started: ', totalItems);
});

connection.on('processed', function (currentItem, totalItems) {
    console.info('loading -> processed: ', currentItem);
});

connection.on('finished', function () {
    console.info('loading -> finished');
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
