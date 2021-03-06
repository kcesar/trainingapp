import React, { Component } from 'react'
import './step.css'

export default class Step extends Component {
  render() {
    return (
      <section className='step'>
        <div>
          <div className='step-circle'>{this.props.step}</div>
          <div className='step-line hidden-xs-down'></div>
        </div>
        <div>
          <div className='step-title'>{this.props.title}</div>
          <div className='step-body'>{this.props.children}</div>
        </div>
      </section>
    )
  }
}