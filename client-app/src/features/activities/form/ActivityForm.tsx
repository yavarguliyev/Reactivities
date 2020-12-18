import React, { useContext, useEffect, useState } from 'react';
import { Segment, Form, Button, Grid } from 'semantic-ui-react';
import { ActivityFormValues } from '../../../app/models/activity';
import { v4 as uuid } from 'uuid';
import { observer } from 'mobx-react-lite';
import { RouteComponentProps } from 'react-router-dom';
import { Form as FinalForm, Field } from 'react-final-form';
import TextInput from '../../../app/common/form/TextInput';
import TextArea from '../../../app/common/form/TextArea';
import SelectInput from '../../../app/common/form/SelectInput';
import { category } from '../../../app/common/options/categoryOptions';
import DateInput from '../../../app/common/form/DateInput';
import { combineDateAndTime } from '../../../app/common/util/util';
import { combineValidators, composeValidators, hasLengthGreaterThan, isRequired } from 'revalidate';
import { RootStoreContext } from '../../../app/stores/rootStore';

var validate = combineValidators({
  title: isRequired({ message: 'The event title is required' }),
  category: isRequired({ message: 'Category is required' }),
  description: composeValidators(
    isRequired('Description is required'),
    hasLengthGreaterThan(4)('Description needs to be at least 5 characters')
  )(),
  city: isRequired({ message: 'City is required' }),
  venue: isRequired({ message: 'Venue is required' }),
  date: isRequired({ message: 'Date is required' }),
  time: isRequired({ message: 'time is required' }),
});

interface DetailParams {
  id: string;
}

const ActivityForm: React.FC<RouteComponentProps<DetailParams>> = ({ match, history }) => {
  const rootStore = useContext(RootStoreContext);
  const { createActivity, editActivity, submitting, loadActivity, clearActivity } = rootStore.activityStore;

  const [activity, setActivity] = useState(new ActivityFormValues());
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (match.params.id) {
      setLoading(true);
      loadActivity(match.params.id).then((activity) => setActivity(new ActivityFormValues(activity))).finally(() => setLoading(false));
    }
  }, [loadActivity, clearActivity, match.params.id]);

  const handleFinalFormSubmit = (values: any) => {
    const dateAndTime = combineDateAndTime(values.date, values.time);
    const { date, time, ...activity } = values;
    activity.date = dateAndTime;
    if (!activity.id) {
      let newActivity = {
        ...activity,
        id: uuid()
      }
      createActivity(newActivity);
    } else {
      editActivity(activity);
    }
  };

  return (
    <Grid>
      <Grid.Column width={10}>
        <Segment clearing>
          <FinalForm validate={validate} initialValues={activity} onSubmit={handleFinalFormSubmit} render={({ handleSubmit, invalid, pristine }) => (
            <Form onSubmit={handleSubmit} loading={loading}>
              <Field name="title" placeholder="Title" value={activity.title} component={TextInput} />
              <Field name="description" placeholder="Description" rows={3} value={activity.description} component={TextArea} />
              <Field name="category" placeholder="Category" value={activity.category} component={SelectInput} options={category} />
              <Form.Group widths='equal'>
                <Field name="date" date={true} placeholder="Date" value={activity.date} component={DateInput} />
                <Field name="time" time={true} placeholder="Time" value={activity.time} component={DateInput} />
              </Form.Group>
              <Field name="city" placeholder="City" value={activity.city} component={TextInput} />
              <Field name="venue" placeholder="Venue" value={activity.venue} component={TextInput} />
              <Button disabled={loading || invalid || pristine} loading={submitting} floated="right" positive type="submit" content="Submit" />
              <Button disabled={loading} onClick={activity.id ? () => history.push(`/activities/${activity.id}`) : () => history.push(`/activities`)} floated="right" type="button" content="Cancel" />
            </Form>
          )} />
        </Segment>
      </Grid.Column>
    </Grid>
  )
}

export default observer(ActivityForm);
