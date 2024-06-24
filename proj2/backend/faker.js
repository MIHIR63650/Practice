const { faker } = require('@faker-js/faker');
const createCsvWriter = require('csv-writer').createObjectCsvWriter;

const totalRecords = 100;

const csvWriter = createCsvWriter({
  path: 'students_data.csv',
  header: [
    { id: 'name', title: 'Name' },
    { id: 'email', title: 'Email' },
  ],
});

const records = [];

for (let i = 1; i <= totalRecords; i++) {
  records.push({
    name: faker.person.fullName(),
    email: faker.internet.email(),
  });

  if (i % 10000 === 0) {
    console.log(`Generated ${i} records`);
  }
}

csvWriter.writeRecords(records)
  .then(() => {
    console.log('CSV file created successfully.');
  })
  .catch((err) => {
    console.error('Error creating CSV file:', err);
  });
