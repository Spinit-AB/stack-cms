import * as React from 'react';
import { render } from 'react-dom';

class HomePage extends React.Component<any, any>{
    constructor(props: any) {
        super(props);

        this.state = {
            test: "React test string"
        }
    }

    render() {
        return (
            <h1>{this.state.test}</h1>
        );
    }
}

export default HomePage;
