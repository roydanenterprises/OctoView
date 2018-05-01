import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Features } from './components/Features';

export const routes = <Layout>
	<Route exact path='/' component={Features} />

</Layout>;
