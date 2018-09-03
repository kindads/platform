'use strict';
module.exports = function (app) {
    var smartContract = require('../controllers/scController');

    // todoList Routes
    app.route('/scs')
        .get(smartContract.list_all_sc)
        .post(smartContract.create_a_sc);


    app.route('/sc/:scId')
        .get(smartContract.read_a_sc)
        .put(smartContract.update_a_sc)
        .delete(smartContract.delete_a_sc);
};
