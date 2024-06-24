import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css';

function App() {
  const [file, setFile] = useState(null);
  const [responseMessage, setResponseMessage] = useState(null);
  const [fetchedData, setFetchedData] = useState([]);
  const [errorMessage, setErrorMessage] = useState(null);

  useEffect(() => {
    fetchBackendData();
  }, []);

  const handleFileChange = (event) => {
    setFile(event.target.files[0]);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    if (!file) {
      alert('No file selected');
      return;
    }

    // Clear previous error messages
    setErrorMessage(null);

    const data = new FormData();
    data.append('file', file);

    try {
      const reader = new FileReader();
      reader.onload = async (e) => {
        const text = e.target.result;

        const lines = text.split('\n');
        const emails = new Set();

        for (const line of lines) {
          const [email] = line.split(',');
          if (emails.has(email)) {
            setErrorMessage(`Duplicate email found: ${email}. Please check your data.`);
            return;
          }
          emails.add(email);
        }

        const response = await axios.post('http://localhost:3000/submit', data, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        });

        alert('Form data submitted successfully!');
        console.log('Response:', response.data);
        setResponseMessage(response.data);
        fetchBackendData();
      };


    } catch (error) {
      console.error('Error submitting form data:', error);
      alert('Failed to submit form data');
    }
  };

  const fetchBackendData = async () => {
    try {
      const response = await axios.get('http://localhost:3000/fetch');
      console.log('Fetched data:', response.data);
      setFetchedData(response.data.data);
    } catch (error) {
      console.error('Error fetching data:', error);
      alert('Failed to fetch data');
    }
  };

  return (
    <div className="App">
      <form onSubmit={handleSubmit}>
        <div>
          <label>File:</label>
          <input type="file" onChange={handleFileChange} />
        </div>
        <button type="submit">Submit</button>
      </form>

      {errorMessage && (
        <div className="error-message">
          <p>{errorMessage}</p>
          <button onClick={() => setErrorMessage(null)}>Dismiss</button>
        </div>
      )}

      {responseMessage && (
        <div>
          <h3>Response from Server:</h3>
          <pre>{JSON.stringify(responseMessage, null, 2)}</pre>
        </div>
      )}

      <div>
        <h3>Fetched Data from Backend:</h3>
        {fetchedData.length > 0 ? (
          <ul>
            {fetchedData.map((item) => (
              <li key={item.id}>
                {item.name} - {item.email} - {item.contact}
              </li>
            ))}
          </ul>
        ) : (
          <p>No data fetched</p>
        )}
      </div>
    </div>
  );
}

export default App;
