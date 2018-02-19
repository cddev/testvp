import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import Characters from './Characters.js';

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <img src={logo} className="App-logo" alt="logo" />
        </header>        
        <Characters />
        
      </div>
    );
  }
}

export default App;
