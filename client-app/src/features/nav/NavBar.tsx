import { observer } from 'mobx-react-lite';
import React from 'react'
import { NavLink } from 'react-router-dom';
import { Menu, Container, Button } from 'semantic-ui-react';

const NavBar: React.FC = () => {
  return (
    <Menu fixed="top" inverted>
      <Container>
        <Menu.Item
          header
          as={NavLink}
          exact
          to="/">
          <img src="/assets/logo.png" alt="logo" style={{ marginRight: '10px' }} />
          Reactivities
        </Menu.Item>
        <Menu.Item
          as={NavLink}
          to="/activities"
          name='Activities'
        />
        <Menu.Item
          as={NavLink}
          to="/create-activity">
          <Button positive content="Create Reactivities" />
        </Menu.Item>
      </Container>
    </Menu>
  )
}

export default observer(NavBar);
