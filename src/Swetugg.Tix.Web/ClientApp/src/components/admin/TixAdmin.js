import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { actionCreators } from '../../store/TixAdmin';

class TixAdmin extends Component {

    componentDidMount() {
        // This method is called when the component is first added to the document
        this.ensureDataFetched();
    }

    componentDidUpdate() {
        // This method is called when the route parameters change
        this.ensureDataFetched();
    }

    ensureDataFetched() {
        this.props.requestAdmin();
    }

    render(props) {
        return (
            <div>
                <h1>Admin</h1>
                <div class="card-columns">
                    {renderActivityCard(props)}
                    {renderActivityCard(props)}
                    {renderActivityCard(props)}
                    {renderActivityCard(props)}
                    {renderActivityCard(props)}
                </div>
            </div>
        );
    }
}

function renderActivityCard(props){
    return (                        
    <div class="card">
        <div class="card-header">
            Activity
        </div>
        <div class="card-body">
        <dl class="row">
            <dt class="col-7">Sold</dt>
            <dd class="col">10</dd>
        </dl>
        <dl class="row">
            <dt class="col-7">Tickets (total)</dt>
            <dd class="col">20</dd>
        </dl>
        </div>
    </div>
);
}

export default connect(
    state => state.tixAdmin,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(TixAdmin);
