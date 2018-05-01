import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class Logout extends React.Component<RouteComponentProps<{}>, {}> {
    constructor() {
        super();

    }
    render(){
        return <div>This no longer does  anything????  Logout occurs through the shared layout using MVC post actions.  This will work well i think.  We will seperate login and user management out to the MVC portion of the application.</div>;
    }
    componentDidMount(){
        fetch('account/logout',{
            method:'POST'
        });
    }
}
