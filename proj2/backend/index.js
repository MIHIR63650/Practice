const express = require('express');
const app = express();
const port = 5000;
const cors = require('cors');
app.use(cors());

const multer = require('multer');
const fs = require('fs');
const csv = require('csv-parser');

const upload = multer({ dest: 'uploads/' });


app.get('/', (req, res) => {
    res.send('Hello World!');
});

app.post('/upload', upload.single('file'), (req, res) => {
    fs.createReadStream(req.file.path)
        .pipe(csv())
        .on('data', (row) => {
            console.log(row);
        })
        .on('end', () => {
            res.send('File uploaded and parsed!');
        });
});

app.listen(port, () => {
    console.log(`App listening at http://localhost:${port}`);
});