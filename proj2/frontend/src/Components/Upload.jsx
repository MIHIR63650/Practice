import React, { useState } from "react";
import axios from "axios";

export default function Upload() {
  const [file, setFile] = useState(null);
  const[uploadPercentage, setUploadPercentage] = useState(0);

  const submitFile = () => {
    const formData = new FormData();
    formData.append("file", file);

    axios
      .post("http://localhost:5000/upload", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
        onUploadProgress: (progressEvent) => {
          const { loaded, total } = progressEvent;
          let percent = Math.floor((loaded * 100) / total);
          setUploadPercentage(percent);
        },
      })
      .then((response) => {
        console.log(response.data);
        setTimeout(() => setUploadPercentage(0), 10000);
      })
      .catch((error) => {
        console.error(error);
      });
  };

  return (
    <div>
      <input
        type="file"
        accept=".csv"
        onChange={(e) => setFile(e.target.files[0])}
      />
      <br />
      <button onClick={submitFile}>Submit</button>
      <br />
      {uploadPercentage > 0 && <progress value={uploadPercentage} max="100" />}
    </div>
  );
}
