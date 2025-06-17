using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Core.Repository;

namespace Services.Features.Rating;

public class RatingService : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IRatingRepository _ratingRepository;
    private double _averageRating;
    private bool _hasUserRated;

    public RatingService(IRatingRepository ratingRepository)
    {
        _ratingRepository = ratingRepository;
        Ratings = new ObservableCollection<Core.Entity.Rating>();
    }

    public ObservableCollection<Core.Entity.Rating> Ratings { get; }

    public double AverageRating
    {
        get => _averageRating;
        private set
        {
            if (_averageRating != value)
            {
                _averageRating = value;
                OnPropertyChanged();
            }
        }
    }

    public bool HasUserRated
    {
        get => _hasUserRated;
        private set
        {
            if (_hasUserRated != value)
            {
                _hasUserRated = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task LoadRatingsAsync(int productId)
    {
        Ratings.Clear();
        var ratings = await _ratingRepository.GetRatingsByProductIdAsync(productId);

        foreach (var rating in ratings)
        {
            Ratings.Add(rating);
        }

        CalculateAverageRating();
    }

    public async Task LoadUserRatingAsync(string userId, int productId)
    {
        var userRatings = await _ratingRepository.GetRatingsByUserIdAsync(userId, productId);
        HasUserRated = await _ratingRepository.CheckIfUserHasRatedAsync(userId, productId);
    }

    public async Task AddRatingAsync(Core.Entity.Rating rating)
    {
        await _ratingRepository.AddRatingAsync(rating);
        Ratings.Add(rating);
        CalculateAverageRating();
        HasUserRated = true;
    }

    private void CalculateAverageRating()
    {
        if (Ratings.Count == 0)
        {
            AverageRating = 0;
            return;
        }

        double sum = 0;
        foreach (var rating in Ratings)
        {
            sum += rating.RatingValue;
        }

        AverageRating = sum / Ratings.Count;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}