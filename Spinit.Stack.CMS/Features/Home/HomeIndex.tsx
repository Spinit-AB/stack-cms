import * as React from 'react';
import { render } from 'react-dom';
import HomePage from './HomePage';

let element = document.getElementById('HomePage');

if (element) {
    render(
        <HomePage />,
        element
    );
}