import React, { Component } from 'react';
import './App.css';
import {base_url,public_key,private_key} from './params.js';
import Popup from "reactjs-popup";
import CryptoJS from "crypto-js";

class Characters extends React.Component{
    constructor(props){
        super(props);

        this.state = {
            error:null,
            isLoaded:false,
            items:[]
        };
    }

    componentDidMount(){
           
        //Building Request
        var ts = Math.floor(Date.now()/1000); //timestamp
        var concatStr = ts.toString() + private_key + public_key; 
        var hash = CryptoJS.MD5(concatStr).toString(); 
        
        var rq = base_url + '/v1/public/characters?apikey='+public_key+'&ts='+ts.toString()+'&hash='+hash;
        
        //Init Headers
        var reqHeader = new Headers();
        reqHeader.append('Content-Type','application/json');   
        var reqInit = { method:'GET',
                        Headers:reqHeader
                        };

     
       //Sending Get Request 
       fetch(rq,reqInit)       
        .then(res => res.json())
        .then(
            (result) => {
                this.setState({
                    isLoaded:true,
                    items: result.data.results
                });                
            },
            (error) => {
                this.setState({
                    isLoaded:true,
                    error
                });
            }
        )          
        
    }  
   
    render(){
        const { error, isLoaded, items } = this.state;
        if (error) {
          return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
          return <div>Loading...</div>;
        } else {
          return (
            <div className="heroesList">
              {items.map(item => (
                <div key={item.id} className="hero">
                <img src={item.thumbnail.path + '/portrait_medium.' +item.thumbnail.extension } alt={item.name} />
                  <div className="thumbs">{item.name}<br />
                  
                  <Popup trigger={<button className="btnDetail">Detail</button> }  modal >
                  {close => (
              <div className="modal">
                <a className="close" onClick={close}>
                 &times;
                 </a>
                <div className="header">
                  <h1>{item.name}</h1>
                  <span>{item.description}</span>
                </div>
                <div className="content">
                  <img src={item.thumbnail.path + '/portrait_medium.' +item.thumbnail.extension } alt={item.name} />
                  <p>Comics</p>
                  <ul>
                      {item.comics.items.map(item =>(
                        <li key={item.name}>{item.name}</li>
                      ))}                      
                  </ul> 
                  <p>Series</p>
                  <ul>
                      {item.series.items.map(item =>(
                        <li key={item.name}>{item.name}</li>
                      ))}                      
                  </ul> 
                  </div>   
              </div>
                  )}
            </Popup>
                </div>
                </div>
              ))}
            </div>
          );
        }
        
    }

}

export default Characters