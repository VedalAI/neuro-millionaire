import express from "express";
import {urlencoded} from "body-parser";
import {send} from "./index";

const app = express();
app.use(urlencoded({extended: false}));
app.listen(1337);

app.get("/answer", (req, res) => {
    send({
        Command: "answer",
        Data: {
            answer: req.body.answer
        }
    });
    res.sendStatus(200);
});

app.get("/lifeline", (req, res) => {
    send({
        Command: "lifeline",
        Data: {
            lifeline: req.body.lifeline
        }
    });
    res.sendStatus(200);
})

app.get("/asktheaudience", (req, res) => {
    send({
        Command: "lifeline/ask_the_audience/results",
        Data: {
            "percentageA": parseInt(req.body.a),
            "percentageB": parseInt(req.body.b),
            "percentageC": parseInt(req.body.c),
            "percentageD": parseInt(req.body.d),
        }
    });
    res.sendStatus(200);
});

app.get("/phoneafriend", (req, res) => {
    send({
        Command: "lifeline/phone_a_friend/results",
        Data: {
            "result": req.body.answer
        }
    });
    res.sendStatus(200);
});
