# 09 — Implementation Notes

> Technical guidance, edge cases, data handling, and development considerations

---

## Technology Recommendations

### Frontend Stack

| Layer | Recommendation | Rationale |
|-------|----------------|-----------|
| Framework | React 18+ | Component model, ecosystem |
| State | Zustand or Redux Toolkit | Simple state for dashboards |
| Styling | CSS Modules or Tailwind | Scoped styles, utility classes |
| Charts | Recharts or Victory | React-native, customizable |
| Icons | Lucide React | Consistent, tree-shakeable |

### Alternative Stacks

| Platform | Recommendation |
|----------|----------------|
| .NET | Uno Platform, WinUI 3 |
| Vanilla | Web Components + CSS |
| Vue | Vue 3 + Composition API |

---

## Project Structure

### Recommended Directory Layout

```
src/
├── components/
│   ├── common/
│   │   ├── Button/
│   │   ├── Panel/
│   │   ├── StatusIndicator/
│   │   ├── ProgressBar/
│   │   └── ...
│   ├── layout/
│   │   ├── Header/
│   │   ├── Footer/
│   │   └── PageLayout/
│   └── features/
│       ├── overview/
│       ├── production/
│       ├── analytics/
│       ├── maintenance/
│       └── settings/
├── pages/
│   ├── Overview.tsx
│   ├── Production.tsx
│   ├── Analytics.tsx
│   ├── Maintenance.tsx
│   └── Settings.tsx
├── hooks/
│   ├── usePolling.ts
│   ├── useWebSocket.ts
│   └── useSettings.ts
├── services/
│   ├── api.ts
│   └── websocket.ts
├── stores/
│   └── index.ts
├── styles/
│   ├── tokens.css
│   ├── reset.css
│   └── utilities.css
├── types/
│   └── index.ts
└── utils/
    ├── formatting.ts
    └── validation.ts
```

---

## Data Handling

### API Integration

#### Polling Strategy

```typescript
// usePolling.ts
function usePolling<T>(
  fetcher: () => Promise<T>,
  interval: number = 5000
) {
  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<Error | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    
    const poll = async () => {
      try {
        const result = await fetcher();
        if (mounted) {
          setData(result);
          setError(null);
        }
      } catch (e) {
        if (mounted) setError(e as Error);
      } finally {
        if (mounted) setLoading(false);
      }
    };

    poll(); // Initial fetch
    const id = setInterval(poll, interval);

    return () => {
      mounted = false;
      clearInterval(id);
    };
  }, [fetcher, interval]);

  return { data, error, loading };
}
```

#### WebSocket Connection

```typescript
// websocket.ts
class AlertSocket {
  private ws: WebSocket | null = null;
  private reconnectAttempts = 0;
  private maxReconnects = 5;

  connect(onAlert: (alert: Alert) => void) {
    this.ws = new WebSocket('wss://api.nexus.example/alerts');

    this.ws.onmessage = (event) => {
      const alert = JSON.parse(event.data);
      onAlert(alert);
    };

    this.ws.onclose = () => {
      if (this.reconnectAttempts < this.maxReconnects) {
        setTimeout(() => {
          this.reconnectAttempts++;
          this.connect(onAlert);
        }, 1000 * Math.pow(2, this.reconnectAttempts));
      }
    };

    this.ws.onopen = () => {
      this.reconnectAttempts = 0;
    };
  }

  disconnect() {
    this.ws?.close();
  }
}
```

### Data Refresh Intervals

| Data Type | Interval | Method |
|-----------|----------|--------|
| KPI Metrics | 5 seconds | Polling |
| Production Lines | 5 seconds | Polling |
| Equipment Health | 30 seconds | Polling |
| Alerts | Real-time | WebSocket |
| Analytics | On-demand | Fetch |
| Settings | On-demand | Fetch |

### Stale Data Handling

```typescript
interface DataWithTimestamp<T> {
  data: T;
  fetchedAt: number;
}

function isStale(fetchedAt: number, maxAge: number = 30000): boolean {
  return Date.now() - fetchedAt > maxAge;
}

// In component
const isDataStale = isStale(metrics.fetchedAt);

// Visual indication
<div className={isDataStale ? 'data-stale' : ''}>
  {/* content */}
</div>
```

---

## State Management

### Global State Shape

```typescript
interface AppState {
  // Connection
  connectionStatus: 'connected' | 'connecting' | 'disconnected';
  lastSync: number;
  
  // Data
  metrics: MetricsState;
  productionLines: ProductionLine[];
  batches: Batch[];
  equipment: Equipment[];
  workOrders: WorkOrder[];
  alerts: Alert[];
  spareParts: SparePart[];
  users: User[];
  
  // UI
  activeSection: Section;
  settings: UserSettings;
}

interface MetricsState {
  throughput: MetricValue;
  efficiency: MetricValue;
  uptime: MetricValue;
  energy: MetricValue;
}

interface MetricValue {
  current: number;
  previousPeriod: number;
  status: 'nominal' | 'warning' | 'critical';
}
```

### Local Component State

Keep UI-only state local:

```typescript
// Local state examples
const [isDropdownOpen, setDropdownOpen] = useState(false);
const [editingRowId, setEditingRowId] = useState<string | null>(null);
const [formValues, setFormValues] = useState(initialValues);
```

---

## Error Handling

### Error Types

| Error Type | User Message | Recovery |
|------------|--------------|----------|
| Network | "Unable to connect. Retrying..." | Auto-retry with backoff |
| Auth | "Session expired. Please log in." | Redirect to login |
| Validation | Specific field error | Inline message |
| Server | "Something went wrong. Please try again." | Manual retry |
| Not Found | "Resource not found." | Navigate back |

### Error Boundaries

```tsx
class ErrorBoundary extends React.Component<Props, State> {
  state = { hasError: false, error: null };

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  render() {
    if (this.state.hasError) {
      return (
        <Panel title="Error">
          <p>Something went wrong loading this section.</p>
          <Button onClick={() => this.setState({ hasError: false })}>
            Try Again
          </Button>
        </Panel>
      );
    }

    return this.props.children;
  }
}
```

### API Error Handling

```typescript
async function fetchWithErrorHandling<T>(
  url: string,
  options?: RequestInit
): Promise<T> {
  try {
    const response = await fetch(url, options);
    
    if (!response.ok) {
      if (response.status === 401) {
        // Handle auth error
        throw new AuthError('Session expired');
      }
      throw new ApiError(response.statusText, response.status);
    }
    
    return response.json();
  } catch (error) {
    if (error instanceof TypeError) {
      // Network error
      throw new NetworkError('Unable to connect');
    }
    throw error;
  }
}
```

---

## Form Handling

### Validation Rules

```typescript
const validationRules = {
  temperatureMax: {
    required: true,
    min: 0,
    max: 150,
    type: 'number',
    message: 'Temperature must be between 0 and 150°C'
  },
  pressureMax: {
    required: true,
    min: 0,
    max: 10,
    type: 'number',
    message: 'Pressure must be between 0 and 10 bar'
  },
  email: {
    required: true,
    pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    message: 'Please enter a valid email address'
  },
  name: {
    required: true,
    minLength: 2,
    maxLength: 100,
    message: 'Name must be between 2 and 100 characters'
  }
};
```

### Form State Pattern

```typescript
interface FormState<T> {
  values: T;
  errors: Partial<Record<keyof T, string>>;
  touched: Partial<Record<keyof T, boolean>>;
  isSubmitting: boolean;
  isValid: boolean;
}

function useForm<T>(initialValues: T, validationRules: ValidationRules) {
  const [state, setState] = useState<FormState<T>>({
    values: initialValues,
    errors: {},
    touched: {},
    isSubmitting: false,
    isValid: false
  });

  const validate = useCallback(() => {
    // Validation logic
  }, [state.values]);

  const handleChange = useCallback((field: keyof T, value: any) => {
    setState(prev => ({
      ...prev,
      values: { ...prev.values, [field]: value },
      touched: { ...prev.touched, [field]: true }
    }));
  }, []);

  return { ...state, handleChange, validate };
}
```

### Confirmation Patterns

For destructive actions:

```typescript
function DeleteUserButton({ userId, userName }) {
  const [showConfirm, setShowConfirm] = useState(false);
  
  return (
    <>
      <Button onClick={() => setShowConfirm(true)}>Remove</Button>
      
      {showConfirm && (
        <ConfirmDialog
          title="Remove User"
          message={`Are you sure you want to remove ${userName}?`}
          confirmLabel="Remove"
          cancelLabel="Cancel"
          onConfirm={() => deleteUser(userId)}
          onCancel={() => setShowConfirm(false)}
          destructive
        />
      )}
    </>
  );
}
```

---

## Performance Optimization

### Rendering Optimization

```typescript
// Memoize expensive components
const MetricCard = React.memo(function MetricCard({ metric }) {
  // ...
});

// Memoize callbacks
const handleClick = useCallback(() => {
  // ...
}, [dependencies]);

// Memoize computed values
const sortedData = useMemo(() => {
  return data.sort((a, b) => a.name.localeCompare(b.name));
}, [data]);
```

### Virtual Scrolling

For long lists (alerts, logs):

```typescript
import { FixedSizeList } from 'react-window';

function AlertList({ alerts }) {
  return (
    <FixedSizeList
      height={400}
      itemCount={alerts.length}
      itemSize={44}
      width="100%"
    >
      {({ index, style }) => (
        <AlertItem
          style={style}
          alert={alerts[index]}
        />
      )}
    </FixedSizeList>
  );
}
```

### Code Splitting

```typescript
// Lazy load pages
const Analytics = React.lazy(() => import('./pages/Analytics'));
const Maintenance = React.lazy(() => import('./pages/Maintenance'));
const Settings = React.lazy(() => import('./pages/Settings'));

// In router
<Suspense fallback={<PageSkeleton />}>
  <Routes>
    <Route path="/analytics" element={<Analytics />} />
    <Route path="/maintenance" element={<Maintenance />} />
    <Route path="/settings" element={<Settings />} />
  </Routes>
</Suspense>
```

---

## Edge Cases

### Empty States

| Context | Message | Action |
|---------|---------|--------|
| No production lines | "No production lines configured." | Contact admin |
| No batches | "No batches in queue." | + NEW BATCH |
| No alerts | "No recent alerts." | — |
| No work orders | "No scheduled maintenance." | + WORK ORDER |
| No users (impossible) | — | — |

### Loading States

| Context | Loading Indicator |
|---------|-------------------|
| Page load | Full skeleton |
| Data refresh | Subtle indicator (spinner in header) |
| Button action | Button loading state |
| Table load | Row skeletons |

### Connection States

```typescript
function ConnectionStatus({ status }) {
  const config = {
    connected: {
      color: 'success',
      label: 'CONNECTED',
      pulse: true
    },
    connecting: {
      color: 'warning',
      label: 'CONNECTING',
      pulse: true
    },
    disconnected: {
      color: 'danger',
      label: 'DISCONNECTED',
      pulse: false
    }
  };
  
  const { color, label, pulse } = config[status];
  
  return (
    <StatusIndicator color={color} pulse={pulse}>
      {label}
    </StatusIndicator>
  );
}
```

### Boundary Conditions

| Condition | Handling |
|-----------|----------|
| Metric value = 0 | Display "0" (not empty) |
| Metric value > 9999 | Format as "10.0K" |
| Progress > 100% | Cap at 100% display |
| Negative values | Display with minus sign |
| Very long names | Truncate with ellipsis |
| Missing data | Show "—" placeholder |
| Future dates | Display normally |
| Very old dates | Display full date |

---

## Security Considerations

### Authentication

```typescript
// JWT token handling
interface AuthState {
  token: string | null;
  user: User | null;
  expiresAt: number | null;
}

function useAuth() {
  const [auth, setAuth] = useState<AuthState>(loadFromStorage());
  
  const isExpired = auth.expiresAt && Date.now() > auth.expiresAt;
  
  if (isExpired) {
    logout();
  }
  
  return { ...auth, isExpired };
}
```

### Authorization

```typescript
// Permission checks
function usePermission(permission: Permission): boolean {
  const { user } = useAuth();
  
  if (!user) return false;
  
  const rolePermissions: Record<Role, Permission[]> = {
    admin: ['*'],
    supervisor: ['view', 'create_batch', 'create_workorder', 'control_lines', 'export'],
    technician: ['view', 'create_workorder', 'export'],
    operator: ['view']
  };
  
  const permissions = rolePermissions[user.role];
  return permissions.includes('*') || permissions.includes(permission);
}

// Usage
function BatchQueuePanel() {
  const canCreate = usePermission('create_batch');
  
  return (
    <Panel 
      title="Batch Queue"
      action={canCreate && <Button>+ NEW BATCH</Button>}
    >
      {/* ... */}
    </Panel>
  );
}
```

### Input Sanitization

```typescript
// Sanitize user input before display
function sanitize(input: string): string {
  return input
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#x27;');
}

// Or use a library like DOMPurify for rich content
```

---

## Testing Strategy

### Unit Tests

| Component | Test Cases |
|-----------|------------|
| MetricCard | Renders value, shows correct status color, handles loading |
| StatusIndicator | Renders correct color per status, pulses when live |
| Toggle | Toggles on click, respects disabled state |
| Table | Renders rows, handles empty state, sorts correctly |

### Integration Tests

| Flow | Test Steps |
|------|------------|
| Tab Navigation | Click each tab, verify content loads |
| Settings Update | Change toggle, verify persists |
| Alert Handling | Receive WebSocket alert, verify display |
| Form Submission | Fill form, submit, verify feedback |

### E2E Tests

```typescript
// Example Playwright test
test('user can navigate and view data', async ({ page }) => {
  await page.goto('/');
  
  // Verify landing on Overview
  await expect(page.locator('h1')).toContainText('Overview');
  
  // Navigate to Production
  await page.click('text=Production');
  await expect(page.url()).toContain('/production');
  
  // Verify batch queue loads
  await expect(page.locator('.batch-table')).toBeVisible();
});
```

---

## Deployment Checklist

### Pre-Launch

- [ ] All pages render correctly
- [ ] Data polling functional
- [ ] WebSocket connection stable
- [ ] Error states display appropriately
- [ ] Loading states appear
- [ ] Accessibility audit passed
- [ ] Performance audit passed (Lighthouse ≥90)
- [ ] Cross-browser testing complete
- [ ] Responsive testing complete

### Performance Targets

| Metric | Target |
|--------|--------|
| First Contentful Paint | < 1.5s |
| Largest Contentful Paint | < 2.5s |
| Time to Interactive | < 3.0s |
| Cumulative Layout Shift | < 0.1 |
| First Input Delay | < 100ms |

### Monitoring

- Error tracking (Sentry, etc.)
- Performance monitoring (Web Vitals)
- Uptime monitoring
- API health checks
- WebSocket connection monitoring
