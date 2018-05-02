import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Branches } from './components/Branches';

export const routes = <Layout>
	<Route exact path='/' component={Branches} />

</Layout>;
