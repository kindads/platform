'use strict';


exports.list_all_sc = function (req, res) {
    console.log(' list all smart contract');

    var result = {
        name: 'Alberto',
        edad: 5
    }
    res.json(result);
};




exports.create_a_sc = function (req, res) {
    console.log(' create a smart contract');

};


exports.read_a_sc = function (req, res) {
    console.log(' read a smart contract');
    

};


exports.update_a_sc = function (req, res) {
    console.log(' update a smart contract ');
};


exports.delete_a_sc = function (req, res) {
    console.log(' delete a smart contract');
};